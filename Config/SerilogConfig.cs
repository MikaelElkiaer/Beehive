using Serilog;
using System;

namespace beehive.Config
{
    public class SerilogConfig
    {
        public static ILogger CreateLogger()
        {
            return Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProcessId()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("UserDomainName", Environment.UserDomainName)
                .Enrich.WithProperty("UserName", Environment.UserName)
                .WriteTo.ColoredConsole()
                .CreateLogger();
        }
    }
}
