using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Beehive.Utils
{
    public static class SerilogUtils
    {
        public class LogTimer : IDisposable
        {
            private readonly Stopwatch stopwatch;
            private readonly ILogger logger;
            private readonly LogEventLevel logEventLevel;
            private readonly string messageTemplate;
            private readonly object[] propertyValues;

            public LogTimer(ILogger logger, LogEventLevel logEventLevel, string messageTemplate, params object[] propertyValues)
            {
                stopwatch = Stopwatch.StartNew();
                this.logger = logger;
                this.logEventLevel = logEventLevel;
                this.messageTemplate = messageTemplate;
                this.propertyValues = propertyValues;
            }

            public void Dispose()
            {
                stopwatch.Stop();
                logger.ForContext("ElapsedMs", stopwatch.ElapsedMilliseconds)
                    .Write(logEventLevel, $"[{{ElapsedMs}} ms] {messageTemplate}", propertyValues);
            }
        }

        public static LogTimer TimedExecution(this ILogger logger, LogEventLevel logEventLevel, string messageTemplate, params object[] propertyValues)
        {
            return new LogTimer(logger, logEventLevel, messageTemplate, propertyValues);
        }
    }
}
