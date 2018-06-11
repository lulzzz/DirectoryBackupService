using System;
using System.IO;
using System.Security.Cryptography;

namespace DirectoryBackupService.Shared
{
    public static class Files
    {
        public static byte[] SafelyReadFile(string filePath)
        {
            using (var s = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var ms = new MemoryStream())
            {
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static string Hash(byte[] fileContent)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = new MemoryStream(fileContent))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
