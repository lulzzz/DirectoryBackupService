namespace DirectoryBackupService.Shared.Models
{
    internal class SourceSettings
    {
        public SourceSettings(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        string DirectoryPath { get; }
    }
}
