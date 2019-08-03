using System.Threading;

namespace Beehive.Config
{
    public class ProgramContext
    {
        public ProgramContext(CancellationTokenSource cancellationTokenSource)
        {
            CancellationTokenSource = cancellationTokenSource;
        }

        public CancellationTokenSource CancellationTokenSource { get; }
    }
}
