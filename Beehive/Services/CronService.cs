using Beehive.Config;
using Beehive.Model;
using Cronos;
using Serilog;
using System;

namespace Beehive.Services
{
    public class CronService
    {
        private readonly ILogger logger;
        private readonly AppConfig appConfig;
        private readonly RunConfig runConfig;

        public CronService(ILogger logger, AppConfig appConfig, RunConfig runConfig)
        {
            this.logger = logger;
            this.appConfig = appConfig;
            this.runConfig = runConfig;
        }

        public bool ShouldRun(string cronText)
        {
            DateTime? nextOccurence;
            try
            {
                nextOccurence = GetNextOccurence(cronText, runConfig.StartUtc, appConfig.TimeZoneInfo);

            }
            catch (CronParseException ex)
            {
                logger.Warning(ex, "Could not parse cron expression {CronExpression}", cronText);
                return false;
            }

            return IsWithinThreshold(runConfig.StartUtc, nextOccurence, appConfig.RunFrequency);
        }

        internal static DateTime? GetNextOccurence(string cronText, DateTime nowUtc, TimeZoneInfo timeZoneInfo)
        {
            CronExpression cronExpr;
            try
            {
                cronExpr = CronExpression.Parse(cronText);
            }
            catch (Exception ex)
            {
                throw new CronParseException("Failed to parse cron expression", ex);
            }
            return cronExpr.GetNextOccurrence(nowUtc, timeZoneInfo, true);
        }

        internal static bool IsWithinThreshold(DateTime utcNow, DateTime? nextOccurenceUtc, TimeSpan threshold)
        {
            return nextOccurenceUtc.HasValue && (nextOccurenceUtc.Value - utcNow) < threshold;
        }
    }
}
