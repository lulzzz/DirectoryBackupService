using Akka.Actor;

namespace DirectoryBackupService.Shared.Actors
{
    internal sealed class DestinationActor : ReceiveActor
    {
        public sealed class UpsertFileMessage
        {
            public UpsertFileMessage(string filePath)
            {
                FilePath = filePath;
            }

            public string FilePath { get; }
        }

        public sealed class DeleteFileMessage
        {
            public DeleteFileMessage(string filePath)
            {
                FilePath = filePath;
            }

            public string FilePath { get; }
        }

        public sealed class RenameFileMessage
        {
            public RenameFileMessage(string oldFilePath, string newFilePath)
            {
                NewFilePath = newFilePath;
                OldFilePath = oldFilePath;
            }

            public string OldFilePath { get; }

            public string NewFilePath { get; }
        }
    }
}