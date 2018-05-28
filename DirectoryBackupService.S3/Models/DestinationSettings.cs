namespace ConnelHooley.DirectoryBackupService.S3.Models
{
    internal sealed class AwsDestinationSettings
    {
        public AwsDestinationSettings( 
            string awsBucketName, 
            string awsBucketRegion, 
            string awsAccessKeyId, 
            string awsSecretkey) {
            BucketName = awsBucketName;
            BucketRegion = awsBucketRegion;
            AccessKeyId = awsAccessKeyId;
            Secretkey = awsSecretkey;
        }

        public string BucketName { get; }

        public string BucketRegion { get; }

        public string AccessKeyId { get; }

        public string Secretkey { get; }
    }
}
