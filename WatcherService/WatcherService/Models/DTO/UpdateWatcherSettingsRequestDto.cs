using System.ComponentModel.DataAnnotations;

namespace WatcherService.Models.DTO
{
    public class UpdateWatcherSettingsRequestDto
    {
        [Required]
        public string WatchedPath { get; set; }
        [Required]
        public string ProcessedPath { get; set; }
        [Required]
        public string LoggerUrl { get; set; }
        [Required]
        public string Issuer { get; set; }
        [Required]
        public int TokenTTLMinutes { get; set; }
    }
}
