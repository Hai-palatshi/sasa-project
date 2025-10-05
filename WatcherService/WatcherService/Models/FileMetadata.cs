namespace WatcherService.Models
{
    public class FileMetadata
    {
        public string filename { get; set; }
        public string created_at { get; set; }
        public long file_size { get; set; }
        public string hash { get; set; }
    }
}
