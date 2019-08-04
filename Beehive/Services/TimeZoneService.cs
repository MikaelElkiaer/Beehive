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
        private readonly ProgramContext programContext;

        public TimeZoneService(ILogger logger, ProgramContext programContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.programContext = programContext;
        }

        public TimeZoneInfo GetTimeZoneInfo(string tz)
        {
            TimeZoneInfo timeZoneInfo;
            try
            {
                string timeZoneId = tz;
                if (programContext.OperationSystem == OSPlatform.Windows)
                    timeZoneId = TimeZoneConverter.TZConvert.IanaToWindows(tz);

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
