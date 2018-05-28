using ConnelHooley.DirectoryBackupService.Shared;
using static ConnelHooley.DirectoryBackupService.S3.Constants.WindowsService;

namespace ConnelHooley.DirectoryBackupService.S3
{
    internal class Program
    {
        private static int Main(string[] args) => WindowsServiceHost.Run(
            ServiceName,
            DisplayName,
            Description,
            ActorSystemName,
            S3SettingsParser.Parse,
            S3IocModule.Create);
    }
}
