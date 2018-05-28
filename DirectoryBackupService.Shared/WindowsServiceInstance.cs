using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Autofac;
using DirectoryBackupService.Shared.Actors;

namespace ConnelHooley.DirectoryBackupService.Shared
{
    internal class WindowsServiceInstance
    {
        private ActorSystem _actorSystem;
        private IActorRef _rootActor;

        public void Start(string actorSystemName, IContainer container)
        {
            _actorSystem = ActorSystem.Create(actorSystemName, Logging.GetAkkaConfig());
            var resolver = new AutoFacDependencyResolver(container, _actorSystem);
            _rootActor = _actorSystem.ActorOf(resolver.Create<DirectoryActor>());
        }

        public void Stop()
        {
            _actorSystem.Stop(_rootActor);
            _actorSystem.Dispose();
        }
    }
}
