using Beehive.Config;
using Beehive.Utils;
using Docker.DotNet;
using Docker.DotNet.Models;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Beehive.Services
{
    public class ContainerService
    {
        private const string BEEHIVE_ENABLE = "beehive.enable";
        private const string BEEHIVE_CRON = "beehive.cron";
        private const string BEEHIVE_REPLACE_RUNNING = "beehive.replace-running";
        private const string TRUE = "true";
        private const string LABEL = "label";

        private readonly ILogger logger;
        private readonly RunConfig runConfig;
        private readonly DockerClient dockerClient;
        private readonly CronService cronService;

        public ContainerService(ILogger logger, RunConfig runConfig, DockerClient dockerClient, CronService cronService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.runConfig = runConfig ?? throw new ArgumentNullException(nameof(runConfig));
            this.dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            this.cronService = cronService ?? throw new ArgumentNullException(nameof(cronService));
        }

        public async Task Run()
        {
            logger.Information("Run started at [{RunStartUtc:o}]", runConfig.StartUtc);

            using (logger.TimedExecution(LogEventLevel.Information, "Finished run"))
            {
                IList<ContainerListResponse> containers = await GetBeehiveContainers();

                if (containers.Any())
                {
                    logger.Debug("Found {BeehiveEnabledCount} enabled containers", containers.Count());
                    foreach (var c in containers)
                    {
                        logger.Verbose("Determining whether container should run {ImageName} [{ContainerId}]", c.Image, c.ID);
                        if (ShouldRun(c))
                            await Run(c);
                        else
                            logger.Verbose("Container not scheduled for this run {ImageName} [{ContainerId}]", c.Image, c.ID);
                    }
                }
                else
                {
                    logger.Warning("Found no enabled containers");
                }
            }
        }

        private async Task<IList<ContainerListResponse>> GetBeehiveContainers()
        {
            return await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>> { { LABEL, new Dictionary<string, bool> { { $"{BEEHIVE_ENABLE}={TRUE}", true } } } }
            });
        }

        private bool ShouldRun(ContainerListResponse c)
        {
            return c.Labels.TryGetValue(BEEHIVE_CRON, out string cronText) && cronService.ShouldRun(cronText);
        }

        private static bool DetermineReplaceRunning(ContainerListResponse c)
        {
            return c.Labels.TryGetValue(BEEHIVE_REPLACE_RUNNING, out string replaceRunningText) && bool.TryParse(replaceRunningText, out bool replaceRunning) ? replaceRunning : false;
        }

        private async Task Run(ContainerListResponse c)
        {
            var replaceRunning = DetermineReplaceRunning(c);
            if (!IsStopped(c.State))
            {
                logger.Warning("Container not in stopped state {ImageName} [{ContainerId}] - ReplaceRunning is set to {ReplaceRunning}", c.Image, c.ID, replaceRunning);
                if (replaceRunning)
                    await dockerClient.Containers.StopContainerAsync(c.ID, null);
                else
                    return;
            }

            logger.Information("Running container {ImageName} [{ContainerId}]", c.Image, c.ID);
            try
            {
                await dockerClient.Containers.StartContainerAsync(c.ID, null);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to run container {ImageName} [{ContainerId}]", c.Image, c.ID);
            }
        }

        private bool IsStopped(string state)
        {
            switch (state)
            {
                case "created":
                case "exited":
                case "stopped":
                    return true;
                default:
                    return false;
            }
        }
    }
}
