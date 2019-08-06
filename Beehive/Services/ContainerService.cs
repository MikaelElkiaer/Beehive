using Beehive.Config;
using Beehive.Utils;
using Cronos;
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
        private const string BEEHIVE_CRON = "beehive.cron";
        private const string BEEHIVE_ENABLE = "beehive.enable";
        private const string TRUE = "true";
        private const string LABEL = "label";

        private readonly ILogger logger;
        private readonly RunConfig runConfig;
        private readonly DockerClient dockerClient;
        private readonly CronService cronService;

        public ContainerService(ILogger logger, AppConfig appConfig, RunConfig runConfig, DockerClient dockerClient, CronService cronService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.runConfig = runConfig ?? throw new ArgumentNullException(nameof(runConfig));
            this.dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            this.cronService = cronService ?? throw new ArgumentNullException(nameof(cronService));
        }

        public async Task Run()
        {
            logger.Information("Run started at [{RunStartUtc}]", runConfig.StartUtc);

            using (logger.TimedExecution(LogEventLevel.Information, "Finished run"))
            {
                IList<ContainerListResponse> containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>> { { LABEL, new Dictionary<string, bool> { { $"{BEEHIVE_ENABLE}={TRUE}", true } } } }
                });

                if (containers.Any())
                {
                    logger.Debug("Found {BeehiveEnabledCount} enabled containers", containers.Count());
                    foreach (var c in containers)
                    {
                        logger.Verbose("Determining whether container should run {ImageName} [{ContainerId}]", c.Image, c.ID);
                        if (c.Labels.TryGetValue(BEEHIVE_CRON, out string cronText) && cronService.ShouldRun(cronText))
                            await Run(dockerClient, c);
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

        private async Task Run(DockerClient client, ContainerListResponse c)
        {
            logger.Information("Running container {ImageName} [{ContainerId}]", c.Image, c.ID);
            await client.Containers.StartContainerAsync(c.ID, null);
        }
    }
}
