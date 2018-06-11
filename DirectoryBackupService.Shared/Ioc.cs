using Autofac;
using Autofac.Core;
using Autofac.Extras.NLog;
using DirectoryBackupService.Shared.Actors;
using DirectoryBackupService.Shared.Models;

namespace ConnelHooley.DirectoryBackupService.Shared
{
    internal static class Ioc
    {
        public static IContainer Create(SourceSettings sourceSettings, IModule destinationModule)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(sourceSettings);
            builder.RegisterType<SourceActor>().AsSelf();
            builder.RegisterModule<NLogModule>();
            builder.RegisterModule(destinationModule);

            return builder.Build();
        }
    }
}
