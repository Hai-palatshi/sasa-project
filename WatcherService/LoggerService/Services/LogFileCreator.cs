using LoggerService.Models;
using LoggerService.Services.IServices;
using System.Text.RegularExpressions;

namespace LoggerService.Services
{
    public class LogFileCreator : ILogFileCreator
    {
        private readonly IWebHostEnvironment env;
        public LogFileCreator(IWebHostEnvironment _env)
        {
            env = _env;
        }

        public async Task CreateLogFileAsync(FileMetadata metadata)
        {
            Directory.CreateDirectory(Path.Combine(env.ContentRootPath, "logs"));

            var baseName = Path.GetFileNameWithoutExtension(metadata.filename);
            var safeName = Regex.Replace(baseName, @"[^\w\-.]", "_");
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            var fileName = $"{safeName}-{timestamp}.txt";
            var filePath = Path.Combine(env.ContentRootPath, "logs", fileName);

            using (var writer = new StreamWriter(filePath, append: false))
            {
                await writer.WriteLineAsync($"Filename: {metadata.filename}");
                await writer.WriteLineAsync($"Size:  {metadata.file_size}");
                await writer.WriteLineAsync($"Created At: {metadata.created_at}");
            }
        }
        private static string FormatFileSize(long bytes)
        {
            string[] u = { "B", "KB", "MB", "GB", "TB" };
            double s = bytes; int i = 0;
            while (s >= 1024 && i < u.Length - 1)
            {
                s = s / 1024;
                i++;
            }
            return $"{s:0.##} {u[i]}";
        }
    }
}
