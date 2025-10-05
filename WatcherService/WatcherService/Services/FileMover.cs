using System.IO;
using WatcherService.Services.IServices;

namespace WatcherService.Services
{
    public class FileMover : IFileMover
    {
        ILogger<FileMover> log;

        public FileMover(ILogger<FileMover> _log)
        {
            log = _log;
        }
        public void CountFiles_Delete_LIMIT(string directoryPath, int limit)
        {
            if (!Directory.Exists(directoryPath)) return;
            var dir = new DirectoryInfo(directoryPath);
            var files = dir.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(f => f.Name).ToList();
            int excess = files.Count - limit;

            if (excess <= 0) return;

            for (int i = 0; i < excess; i++)
            {
                try
                {
                    files[i].IsReadOnly = false;
                    files[i].Delete();
                    log.LogInformation("Deleted old log file {File}", files[i].FullName);
                }
                catch
                {
                   log.LogError("IO error while deleting old log file {File}", files[i].FullName);
                }
            }
        }

        public void MoveFile(string sourcePath, string destinationPath, string name)
        {
            var targer = Path.Combine(destinationPath, name);
            try
            {
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }
                var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                var dest = Path.Combine(destinationPath, $"{ts}__{name}");
                File.Move(sourcePath, dest);
                log.LogInformation("Moved file {File} to {Target}", sourcePath, dest);
            }
            catch
            {
                log.LogError("IO error while moving file {File} to {Target}", sourcePath, targer);
            }
             
        }
    }
}
