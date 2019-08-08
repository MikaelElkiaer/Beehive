using System;
using System.Threading.Tasks;
using System.Runtime.Loader;
using Serilog;
using Beehive.Config;
using Autofac;
using Beehive.Services;

namespace Beehive
{
    class Program
    {
        static readonly ILifetimeScope container = AutofacConfig.CreateContainer();

        static async Task<int> Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Console.CancelKeyPress += Console_CancelKeyPress;

            try
            {
                while (!container.Resolve<ProgramContext>().CancellationTokenSource.Token.IsCancellationRequested)
                {
                    using (var container = Program.container.BeginLifetimeScope())
                    {
                        await container.Resolve<ContainerService>().Run();
                        await container.Resolve<WaiterService>().Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                container.Resolve<ILogger>().Fatal(ex, "Service crashed...");
                return -1;
            }

            return 0;
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            container.Resolve<ILogger>().Information("Exiting...");
            container.Resolve<ProgramContext>().CancellationTokenSource.Cancel();
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            container.Resolve<ILogger>().Information("Cancelled...");
            container.Resolve<ProgramContext>().CancellationTokenSource.Cancel();
        }
    }
}
