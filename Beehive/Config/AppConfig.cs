using System;

namespace Beehive.Config
{
    public class AppConfig
    {
        public AppConfig(TimeSpan runFrequency)
        {
            RunFrequency = runFrequency;
        }

        public TimeSpan RunFrequency { get; }
    }
}
