using Beehive.Config;
using System;
using System.Threading.Tasks;

namespace Beehive.Services
{
    public class WaiterService
    {
        private readonly AppConfig appConfig;
        private readonly ProgramContext programContext;

        public WaiterService(AppConfig appConfig, ProgramContext programContext)
        {
            this.appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            this.programContext = programContext ?? throw new ArgumentNullException(nameof(programContext));
        }

        public async Task Wait()
        {
            try
            {
                await Task.Delay(appConfig.RunFrequency, programContext.CancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }
}
