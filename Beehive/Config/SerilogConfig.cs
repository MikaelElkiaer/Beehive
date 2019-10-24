using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;

namespace Beehive.Config
{
    public class SerilogConfig
    {
        private static LogEventLevel LOG_LEVEL_DEFAULT = LogEventLevel.Information;

        public static ILogger CreateLogger(string logLevel)
        {
            return Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(GetLogLevel(logLevel))
                .WriteTo.Console(new JsonFormatter(), standardErrorFromLevel: LogEventLevel.Error)
                .CreateLogger();
        }

        private static LogEventLevel GetLogLevel(string logLevel)
        {
            if (Enum.TryParse(logLevel, true, out LogEventLevel parsedLogLevel))
                return parsedLogLevel;
            
            return LOG_LEVEL_DEFAULT;
        }
    }
}
