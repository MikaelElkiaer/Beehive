using System.Runtime.InteropServices;
using System.Threading;

namespace Beehive.Config
{
    public class ProgramContext
    {
        public ProgramContext(CancellationTokenSource cancellationTokenSource, OSPlatform operationSystem)
        {
            CancellationTokenSource = cancellationTokenSource;
            OperationSystem = operationSystem;
        }

        public CancellationTokenSource CancellationTokenSource { get; }
        public OSPlatform OperationSystem { get; }
    }
}
