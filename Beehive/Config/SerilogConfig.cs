using Serilog;
using Serilog.Events;
using System;

namespace Beehive.Config
{
    public class SerilogConfig
    {
        private static LogEventLevel LOG_LEVEL_DEFAULT = LogEventLevel.Information;

        public static ILogger CreateLogger()
        {
            return Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(GetLogLevel())
                .WriteTo.Console(standardErrorFromLevel: LogEventLevel.Error)
                .CreateLogger();
        }

        private static LogEventLevel GetLogLevel()
        {
            return Enum.TryParse(typeof(LogEventLevel), Environment.GetEnvironmentVariable("LOG_LEVEL") ?? LOG_LEVEL_DEFAULT.ToString(), true, out object logLevel)
                ? (LogEventLevel)logLevel : LOG_LEVEL_DEFAULT;
        }
    }
}
