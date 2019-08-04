using System;
using System.Runtime.InteropServices;

namespace Beehive.Config
{
    public class AppConfig
    {
        public AppConfig(TimeSpan runFrequency, TimeZoneInfo timeZoneInfo)
        {
            RunFrequency = runFrequency;
            TimeZoneInfo = timeZoneInfo;
        }

        public TimeSpan RunFrequency { get; }
        public TimeZoneInfo TimeZoneInfo { get; }
    }
}
