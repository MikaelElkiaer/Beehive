using System;

namespace Beehive.Config
{
    public class AppConfig
    {
        public AppConfig(TimeSpan runFrequency, DateTime runStartUtc)
        {
            RunFrequency = runFrequency;
            RunStartUtc = runStartUtc;
        }

        public TimeSpan RunFrequency { get; }
        public DateTime RunStartUtc { get; }
    }
}
