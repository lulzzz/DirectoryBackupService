using Akka.Actor;
using Akka.DI.AutoFac;
using Autofac;
using DirectoryBackupService.Shared.Actors;
using System.Text.RegularExpressions;
using Topshelf.Runtime;

namespace ConnelHooley.DirectoryBackupService.Shared
{
    internal sealed class WindowsServiceInstance
    {
        private readonly HostSettings _hostSettings;
        private readonly IContainer _iocContainer;

        private ActorSystem _actorSystem;
        private IActorRef _rootActor;
        
        public WindowsServiceInstance(HostSettings hostSettings, IContainer iocContainer)
        {
            _hostSettings = hostSettings;
            _iocContainer = iocContainer;
        }

        public void Start()
        {
            _actorSystem = ActorSystem.Create(ParseActorSystemName(), Logging.GetAkkaLoggingConfig());
            var resolver = new AutoFacDependencyResolver(_iocContainer, _actorSystem);
            _rootActor = _actorSystem.ActorOf(resolver.Create<SourceActor>(), "source");

            string ParseActorSystemName()
            {
                var hyphonated = Regex.Replace(_hostSettings.ServiceName, @"[\s|_]+", "-");
                var filtered = Regex.Replace(hyphonated, "[^A-z0-9]", string.Empty);
                return filtered.ToLower();
            }
        }

        public void Stop()
        {
            _actorSystem.Stop(_rootActor);
            _actorSystem.Dispose();
        }
    }
}