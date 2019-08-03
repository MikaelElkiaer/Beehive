using System;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cronos;
using System.Threading;
using System.Runtime.Loader;
using Serilog;
using System.Linq;

namespace beehive
{
    class Program
    {
        const string LINUX = "unix:///var/run/docker.sock";
        const string WINDOWS = "npipe://./pipe/docker_engine";
        private const string BEEHIVE_CRON = "beehive.cron";
        private const string BEEHIVE_ENABLE = "beehive.enable";
        private const string TRUE = "true";
        private const string LABEL = "label";
        private const double TIMER_THRESHOLD_MINUTES = 1d;

        static readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        static ILogger logger;

        static async Task Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += Default_Unloading;
            Console.CancelKeyPress += CancelHandler;

            logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProcessId()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("UserDomainName", Environment.UserDomainName)
                .Enrich.WithProperty("UserName", Environment.UserName)
                .WriteTo.ColoredConsole()
                .CreateLogger();

            while (!tokenSource.Token.IsCancellationRequested)
            {
                logger.Debug("Running at [{RunTimeUtc}]", DateTime.UtcNow);

                Uri dockerEndpoint;
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                    dockerEndpoint = new Uri(WINDOWS);
                else
                    dockerEndpoint = new Uri(LINUX);

                using (DockerClient client = new DockerClientConfiguration(dockerEndpoint)
                    .CreateClient())
                {
                    IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(new ContainersListParameters
                    {
                        All = true,
                        Filters = new Dictionary<string, IDictionary<string, bool>> { { LABEL, new Dictionary<string, bool> { { $"{BEEHIVE_ENABLE}={TRUE}", true } } } }
                    });

                    if (containers.Any())
                    {
                        logger.Debug("Found {BeehiveEnabledCount} enabled containers", containers.Count());
                        foreach (var c in containers)
                            if (ShouldRun(c))
                                await Run(client, c);
                    }
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(TIMER_THRESHOLD_MINUTES), tokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    continue;
                }
            }
        }

        private static void Default_Unloading(AssemblyLoadContext obj)
        {
            logger.Information("Exiting...");
            tokenSource.Cancel();
        }

        private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            logger.Information("Cancelled...");
            tokenSource.Cancel();
        }

        private static bool ShouldRun(ContainerListResponse c)
        {
            c.Labels.TryGetValue(BEEHIVE_CRON, out string cronText);
            var cronExpr = CronExpression.Parse(cronText);
            var utcNow = DateTime.UtcNow;

            var nextOccurence = cronExpr.GetNextOccurrence(utcNow, true);

            return nextOccurence.HasValue && IsWithinThreshold(utcNow, nextOccurence);
        }

        private static bool IsWithinThreshold(DateTime utcNow, DateTime? nextOccurence)
        {
            return (utcNow - nextOccurence.Value).TotalMinutes < TIMER_THRESHOLD_MINUTES;
        }

        private static async Task Run(DockerClient client, ContainerListResponse c)
        {
            logger.Information("Running container [{ImageName}] - [{ContainerId}]", c.Image, c.ID);
            await client.Containers.StartContainerAsync(c.ID, null);
        }
    }
}
