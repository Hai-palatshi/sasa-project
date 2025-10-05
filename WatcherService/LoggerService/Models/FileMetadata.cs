using System.ComponentModel.DataAnnotations;

namespace LoggerService.Models
{
    public class FileMetadata
    {
        [Required]
        public string filename { get; set; }
        [Required]
        public string created_at { get; set; }
        [Required]
        public long file_size { get; set; }
        [Required]
        public string hash { get; set; }

    }
}
