using ConnelHooley.DirectoryBackupService.Shared;
using static ConnelHooley.DirectoryBackupService.S3.Constants.WindowsService;

namespace ConnelHooley.DirectoryBackupService.S3
{
    internal static class Program
    {
        private static int Main(string[] args) => WindowsServiceHost.Run(
            ServiceName,
            DisplayName,
            Description,
            S3SettingsParser.Parse,
            S3IocModule.Create);
    }
}
