using System;
using System.Threading.Tasks;

namespace ConnelHooley.DirectoryBackupService.S3.Utilities
{
    public sealed class S3Client : IS3Client
    {
        private readonly Amazon.S3.AmazonS3Client _s3Client;
        private readonly string _bucketName;
        private readonly string _sourceDirectory;

        public S3Client(Amazon.S3.AmazonS3Client s3Client, string bucketName, string sourceDirectory)
        {
            _s3Client = s3Client;
            _bucketName = bucketName;
            _sourceDirectory = sourceDirectory;
        }

        public async Task RenameFileAsync(string sourceFilePath, string destinationFilePath)
        {
            var sourceKey = PathRelativeToSourceDirectory(sourceFilePath);
            var destinationKey = PathRelativeToSourceDirectory(destinationFilePath);
            await _s3Client.CopyObjectAsync(_bucketName, sourceKey, _bucketName, destinationKey).ConfigureAwait(false);
            await _s3Client.DeleteObjectAsync(_bucketName, sourceKey).ConfigureAwait(false);
        }

        public async Task UpsertFileAsync(string filePath)
        {
            var key = PathRelativeToSourceDirectory(filePath);
            await _s3Client.PutObjectAsync(new Amazon.S3.Model.PutObjectRequest {
                Key = key,
                BucketName = _bucketName
            }).ConfigureAwait(false);
        }

        public async Task DeleteFileAsync(string filePath)
        {
            var key = PathRelativeToSourceDirectory(filePath);
            await _s3Client.DeleteObjectAsync(_bucketName, key).ConfigureAwait(false);
        }

        private string PathRelativeToSourceDirectory(string filePath) =>
            new Uri(filePath).MakeRelativeUri(new Uri(_sourceDirectory)).ToString();
    }
}
