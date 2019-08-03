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

        static async Task Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += Default_Unloading;
            Console.CancelKeyPress += CancelHandler;

            while (!container.Resolve<ProgramContext>().CancellationTokenSource.Token.IsCancellationRequested)
            {
                using (var container = Program.container.BeginLifetimeScope())
                {
                    await container.Resolve<ContainerService>().Run();
                    await container.Resolve<WaiterService>().Wait();
                }
            }
        }

        private static void Default_Unloading(AssemblyLoadContext obj)
        {
            container.Resolve<ILogger>().Information("Exiting...");
            container.Resolve<ProgramContext>().CancellationTokenSource.Cancel();
        }

        private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            container.Resolve<ILogger>().Information("Cancelled...");
            container.Resolve<ProgramContext>().CancellationTokenSource.Cancel();
        }
    }
}
