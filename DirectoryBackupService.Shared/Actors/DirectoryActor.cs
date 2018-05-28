using Akka.Actor;
using Akka.Event;
using ConnelHooley.DirectoryBackupService.Shared;
using System;

namespace DirectoryBackupService.Shared.Actors
{
    class DirectoryActor : ReceiveActor
    {
        public DirectoryActor()
        {
            var akkaLog = Logs.GetAkkaLogger(Context);
            akkaLog.Log(LogLevel.DebugLevel, nameof(akkaLog));
            Console.ReadLine();
        }
    }
}
