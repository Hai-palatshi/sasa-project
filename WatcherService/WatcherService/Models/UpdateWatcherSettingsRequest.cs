using System.ComponentModel.DataAnnotations;

namespace WatcherService.Models
{
    public class UpdateWatcherSettingsRequest
    {
        public  string WatchedPath { get; set; } = "watched";
        public  string ProcessedPath { get; set; } = "processed";
        public string LoggerUrl { get; set; } = "https://localhost:5000/api/logger/log";
        public  string Issuer { get; set; } = "watcher-service";
        public  int TokenTTLMinutes { get; set; } = 5;
    }
}
