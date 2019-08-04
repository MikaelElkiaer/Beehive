using Autofac;
using Beehive.Services;
using Docker.DotNet;
using Serilog;
using System;
using System.Threading;

namespace Beehive.Config
{
    public class AutofacConfig
    {
        const string LINUX = "unix:///var/run/docker.sock";
        const string WINDOWS = "npipe://./pipe/docker_engine";
        private const string TZ = "TZ";
        private const string TZ_DEFAULT = "UTC";

        public static ILifetimeScope CreateContainer()
        {
            var cb = new ContainerBuilder();

            RegisterLogger(cb);
            RegisterConfig(cb);
            RegisterServices(cb);

            return cb.Build();
        }

        private static void RegisterLogger(ContainerBuilder cb)
        {
            cb.Register(c => SerilogConfig.CreateLogger()).AsSelf();
        }

        private static void RegisterConfig(ContainerBuilder cb)
        {
            cb.Register(c => new ProgramContext(new CancellationTokenSource())).AsSelf().SingleInstance();
            cb.Register(c => new AppConfig(
                runFrequency: TimeSpan.FromMinutes(1),
                timeZoneInfo: c.Resolve<TimeZoneService>().GetTimeZoneInfo(Environment.GetEnvironmentVariable(TZ) ?? TZ_DEFAULT))
            ).AsSelf().SingleInstance();
            cb.Register(c => new RunConfig(DateTime.UtcNow)).AsSelf().InstancePerLifetimeScope();
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
            cb.RegisterType<CronService>().AsSelf();
            cb.RegisterType<TimeZoneService>().AsSelf();
            cb.RegisterType<WaiterService>().AsSelf();
        }
    }
}
