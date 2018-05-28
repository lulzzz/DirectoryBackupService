namespace ConnelHooley.DirectoryBackupService.S3
{
    internal static class Constants
    {
        internal static class WindowsService
        {
            public const string ServiceName = "DirectoryBackupService.S3";

            public const string DisplayName = "Directory Backup Service (AWS S3 Bucket)";

            public const string Description = "Backs up a directory to an AWS S3 Bucket";

            public const string ActorSystemName = "directory-backup-service-s3";
        }
    }
}
