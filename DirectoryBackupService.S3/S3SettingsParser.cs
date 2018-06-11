using ConnelHooley.DirectoryBackupService.S3.Models;
using DirectoryBackupService.Shared.Models;
using Topshelf.HostConfigurators;

namespace ConnelHooley.DirectoryBackupService.S3
{
    internal static class S3SettingsParser
    {
        public static (SourceSettings, AwsDestinationSettings) Parse(HostConfigurator x)
        {
            string sourceDirectoryPath = string.Empty;
            int sourceBufferSecs = 2;
            string awsBucketName = string.Empty;
            string awsBucketRegion = string.Empty;
            string awsAccessKeyId = string.Empty;
            string awsSecretkey = string.Empty;

            x.AddCommandLineDefinition(nameof(sourceDirectoryPath), v => sourceDirectoryPath = v);
            x.AddCommandLineDefinition(nameof(sourceBufferSecs), v => sourceBufferSecs = int.Parse(v));
            x.AddCommandLineDefinition(nameof(awsBucketName), v => awsBucketName = v);
            x.AddCommandLineDefinition(nameof(awsBucketRegion), v => awsBucketRegion = v);
            x.AddCommandLineDefinition(nameof(awsAccessKeyId), v => awsAccessKeyId = v);
            x.AddCommandLineDefinition(nameof(awsSecretkey), v => awsSecretkey = v);
            x.ApplyCommandLine();

            var sourceSettings = new SourceSettings(sourceDirectoryPath, sourceBufferSecs);

            var destinationSettings = new AwsDestinationSettings(
                awsBucketName,
                awsBucketRegion,
                awsAccessKeyId,
                awsSecretkey);

            return (sourceSettings, destinationSettings);
        }
    }
}
