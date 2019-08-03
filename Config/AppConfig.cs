using System;

namespace beehive.Config
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
