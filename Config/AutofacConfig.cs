using Autofac;
using beehive.Services;
using Docker.DotNet;
using System;
using System.Threading;

namespace beehive.Config
{
    public class AutofacConfig
    {
        const string LINUX = "unix:///var/run/docker.sock";
        const string WINDOWS = "npipe://./pipe/docker_engine";

        public static ILifetimeScope CreateContainer()
        {
            var cb = new ContainerBuilder();

            RegisterLogger(cb);
            RegisterConfig(cb);
            RegisterServices(cb);

            return cb.Build();
        }

        private static void RegisterConfig(ContainerBuilder cb)
        {
            cb.Register(c => new ProgramContext(new CancellationTokenSource())).AsSelf().SingleInstance();
            cb.Register(c => new AppConfig(TimeSpan.FromMinutes(1))).AsSelf().SingleInstance();
        }

        private static void RegisterServices(ContainerBuilder cb)
        {
            Uri dockerEndpoint;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                dockerEndpoint = new Uri(WINDOWS);
            else
                dockerEndpoint = new Uri(LINUX);

            cb.Register(c => new DockerClientConfiguration(dockerEndpoint).CreateClient()).AsSelf().SingleInstance();
            cb.RegisterType<ContainerService>().AsSelf();
            cb.RegisterType<WaiterService>().AsSelf();
        }

        private static void RegisterLogger(ContainerBuilder cb)
        {
            cb.Register(c => SerilogConfig.CreateLogger()).AsSelf();
        }
    }
}
