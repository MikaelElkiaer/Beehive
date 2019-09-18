using System;

namespace Beehive.Config
{
    public class RunConfig
    {
        public RunConfig(DateTime startUtc)
        {
            StartUtc = startUtc;
        }

        public DateTime StartUtc { get; }
    }
}
