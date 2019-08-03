using Beehive.Config;
using Cronos;
using Docker.DotNet;
using Docker.DotNet.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beehive.Services
{
    public class ContainerService
    {
        private const string BEEHIVE_CRON = "beehive.cron";
        private const string BEEHIVE_ENABLE = "beehive.enable";
        private const string TRUE = "true";
        private const string LABEL = "label";

        private readonly ILogger logger;
        private readonly AppConfig appConfig;
        private readonly DockerClient dockerClient;

        public ContainerService(ILogger logger, AppConfig appConfig, DockerClient dockerClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            this.dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
        }

        public async Task Run()
        {
            logger.Debug("Running at [{RunTimeUtc}]", DateTime.UtcNow);

            IList<ContainerListResponse> containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>> { { LABEL, new Dictionary<string, bool> { { $"{BEEHIVE_ENABLE}={TRUE}", true } } } }
            });

            if (containers.Any())
            {
                logger.Debug("Found {BeehiveEnabledCount} enabled containers", containers.Count());
                foreach (var c in containers)
                    if (ShouldRun(c))
                        await Run(dockerClient, c);
            }
        }

        private bool ShouldRun(ContainerListResponse c)
        {
            c.Labels.TryGetValue(BEEHIVE_CRON, out string cronText);
            var cronExpr = CronExpression.Parse(cronText);
            var utcNow = DateTime.UtcNow;

            var nextOccurence = cronExpr.GetNextOccurrence(utcNow, true);

            return nextOccurence.HasValue && IsWithinThreshold(utcNow, nextOccurence);
        }

        private bool IsWithinThreshold(DateTime utcNow, DateTime? nextOccurence)
        {
            return (utcNow - nextOccurence.Value) < appConfig.RunFrequency;
        }

        private async Task Run(DockerClient client, ContainerListResponse c)
        {
            logger.Information("Running container [{ImageName}] - [{ContainerId}]", c.Image, c.ID);
            await client.Containers.StartContainerAsync(c.ID, null);
        }
    }
}
