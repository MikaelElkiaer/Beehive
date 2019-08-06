using Beehive.Config;
using Serilog;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Beehive.Services
{
    public class TimeZoneService
    {
        private readonly ILogger logger;

        public TimeZoneService(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TimeZoneInfo GetTimeZoneInfoWithUtcFallback(string tz)
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.Utc;
            try
            {
                timeZoneInfo = GetTimeZoneInfo(tz ?? "UTC");
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "Could not set timezone from id {TimeZoneId}", tz);
            }

            logger.Debug("Set time zone to {TimeZoneId}", timeZoneInfo.Id);
            return timeZoneInfo;
        }

        public static TimeZoneInfo GetTimeZoneInfo(string tz)
        {
            return TimeZoneConverter.TZConvert.GetTimeZoneInfo(tz);
        }
    }
}
