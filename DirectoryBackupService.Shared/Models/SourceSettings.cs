namespace DirectoryBackupService.Shared.Models
{
    internal sealed class SourceSettings
    {
        public SourceSettings(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        public string DirectoryPath { get; }
    }
}
