using Autofac.Core;
using DirectoryBackupService.Shared.Models;
using System;
using Topshelf;
using Topshelf.HostConfigurators;

namespace ConnelHooley.DirectoryBackupService.Shared
{
    internal static class WindowsServiceHost
    {
        public static int Run<TDestinationSettings>(
            string serviceName, 
            string displayName, 
            string description, 
            Func<HostConfigurator, (SourceSettings, TDestinationSettings)> settingsParser, 
            Func<TDestinationSettings, IModule> destinationIocModuleCreator) =>
                (int) HostFactory.Run(x =>
                {
                    var (sourceSettings, destinationSettings) = settingsParser(x);
                    var destinationIocModule = destinationIocModuleCreator(destinationSettings);
                    var iocContainer = Ioc.Create(sourceSettings, destinationIocModule);
                    Logging.AddToTopShelf(x);

                    x.RunAsLocalSystem();
                    x.StartAutomatically();
                    x.SetServiceName(serviceName);
                    x.SetDisplayName(displayName);
                    x.SetDescription(description);
                    x.Service<WindowsServiceInstance>(s =>
                    {
                        s.ConstructUsing(hs => new WindowsServiceInstance(hs, iocContainer));
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                    });
                });
    }
}
