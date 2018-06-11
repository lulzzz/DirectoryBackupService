using System.Threading.Tasks;

namespace ConnelHooley.DirectoryBackupService.S3.Utilities
{
    public interface IS3Client
    {
        Task RenameFileAsync(string sourceFilePath, string destinationFilePath);

        Task UpsertFileAsync(string filePath);

        Task DeleteFileAsync(string filePath);
    }
}
