using System;

namespace DirectoryBackupService.Shared.Models
{
    internal sealed class SourceSettings
    {
        public SourceSettings(string directoryPath, string bufferSecs)
        {
            DirectoryPath = directoryPath;
            BufferDuration = TimeSpan.FromSeconds(double.Parse(bufferSecs));
        }

        public string DirectoryPath { get; }

        public TimeSpan BufferDuration { get; }
    }
}
