using Serilog;
using System;
using System.Collections.Generic;
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

        public TimeZoneInfo GetTimeZoneInfo(string tz)
        {
            TimeZoneInfo timeZoneInfo;
            try
            {
                var timeZoneId = TimeZoneConverter.TZConvert.IanaToWindows(tz);
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Could not set timezone from id {TimeZoneId}", tz);
                timeZoneInfo = TimeZoneInfo.Utc;
            }

            logger.Verbose("Set time zone to {TimeZoneId}", timeZoneInfo.Id);
            return timeZoneInfo;
        }
    }
}
