using Beehive.Config;
using Cronos;
using System;

namespace Beehive.Services
{
    public class CronService
    {
        private readonly AppConfig appConfig;

        public CronService(AppConfig appConfig)
        {
            this.appConfig = appConfig;
        }

        public bool ShouldRun(string cronText)
        {
            var cronExpr = CronExpression.Parse(cronText);
            var utcNow = appConfig.RunStartUtc;

            var nextOccurence = cronExpr.GetNextOccurrence(utcNow, true);

            return IsWithinThreshold(utcNow, nextOccurence);
        }

        public bool IsWithinThreshold(DateTime utcNow, DateTime? nextOccurence)
        {
            return nextOccurence.HasValue && (nextOccurence.Value - utcNow) < appConfig.RunFrequency;
        }
    }
}
