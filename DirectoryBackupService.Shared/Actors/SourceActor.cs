using Akka.Actor;
using Akka.DI.Core;
using ConnelHooley.DirectoryBackupService.Shared;
using DirectoryBackupService.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace DirectoryBackupService.Shared.Actors
{
    internal sealed class SourceActor : ReceiveActor
    {
        private readonly Akka.Event.ILoggingAdapter _logger;
        private readonly SourceSettings _sourceSettings;
        private IDisposable _fileSystemWatcherSubscription;
        private FileSystemWatcher _fileSystemWatcher;
        private IActorRef _destinationActor;

        public SourceActor(SourceSettings sourceSettings)
        {
            _logger = Logging.GetAkkaLogger(Context);
            _sourceSettings = sourceSettings;
        }

        protected override void PreStart()
        {
            _destinationActor = Context.ActorOf(Context.DI().Props<DestinationActor>(), "destination");

            _fileSystemWatcher = new FileSystemWatcher(_sourceSettings.DirectoryPath, "*")
            {
                NotifyFilter =
                      NotifyFilters.CreationTime
                    | NotifyFilters.FileName
                    | NotifyFilters.LastWrite
            };

            IObservable<FileSystemEventArgs> WatcherEventToObservable(string eventName) => Observable
                .FromEventPattern<FileSystemEventArgs>(
                    _fileSystemWatcher,
                    eventName)
                .Select(e => e.EventArgs);

            _fileSystemWatcherSubscription = Observable
                .Merge(
                    WatcherEventToObservable(nameof(_fileSystemWatcher.Created)),
                    WatcherEventToObservable(nameof(_fileSystemWatcher.Changed)),
                    WatcherEventToObservable(nameof(_fileSystemWatcher.Deleted)),
                    WatcherEventToObservable(nameof(_fileSystemWatcher.Renamed)))
                .GroupBy(e => e.FullPath)
                .Select(g => g.Buffer(g.Throttle(_sourceSettings.BufferDuration)))
                .SelectMany(e => e) // Flattern group
                .SelectMany(argList => // Flattern buffered list
                {
                    if (argList.Count == 1)
                    {
                        return argList;
                    }
                    if (argList.Any(arg => arg.ChangeType == WatcherChangeTypes.Renamed))
                    {
                        var result = new List<FileSystemEventArgs>();
                        for (int i = 1; i < argList.Count; i++)
                        {
                            var previous = argList[i-1];
                            var current = argList[i];
                            if(current.ChangeType == WatcherChangeTypes.Renamed)
                            {
                                result.Add(previous);
                                result.Add(current);
                            }
                        }
                        return result;
                    }
                    var first = argList[0];
                    var last = argList[argList.Count-1];
                    if (first.ChangeType == WatcherChangeTypes.Created && last.ChangeType == WatcherChangeTypes.Deleted)
                    {
                        return new List<FileSystemEventArgs>();
                    }
                    return new List<FileSystemEventArgs> { last };
                })
                .Subscribe(args => {
                    switch (args.ChangeType)
                    {
                        case WatcherChangeTypes.Created:
                        case WatcherChangeTypes.Changed:
                            _destinationActor.Tell(new DestinationActor.UpsertFileMessage(
                                args.FullPath,
                                Files.SafelyReadFile(args.FullPath)));
                            break;
                        case WatcherChangeTypes.Deleted:
                            _destinationActor.Tell(new DestinationActor.DeleteFileMessage(
                                args.FullPath));
                            break;
                        case WatcherChangeTypes.Renamed:
                            if(args is RenamedEventArgs renameArgs)
                            {
                                _destinationActor.Tell(new DestinationActor.RenameFileMessage(
                                    renameArgs.OldFullPath,
                                    renameArgs.FullPath));
                            }
                            break;
                    }
                });

            _fileSystemWatcher.EnableRaisingEvents = true;

            base.PreStart();

            Context.Parent.Tell(new SetUpCompleteMessage());
        }

        protected override void PostStop()
        {
            _fileSystemWatcher.Dispose();
            _fileSystemWatcherSubscription.Dispose();
            base.PostStop();
        }

        public sealed class SetUpCompleteMessage { }
    }
}