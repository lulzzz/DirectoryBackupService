using Akka.Actor;
using ConnelHooley.DirectoryBackupService.Shared;
using DirectoryBackupService.Shared.Models;
using Newtonsoft.Json;
using System;

namespace DirectoryBackupService.Shared.Actors
{
    class DirectoryActor : ReceiveActor
    {
        private Akka.Event.ILoggingAdapter _logger = Logging.GetAkkaLogger(Context);

        public DirectoryActor(SourceSettings sourceSettings)
        {
            _logger.Debug(JsonConvert.SerializeObject(sourceSettings));
        }
    }
}
