namespace WatcherService.Services.IServices
{
    public interface IFileMover
    {
        public void MoveFile(string sourcePath, string destinationPath,string name);
        public void CountFiles_Delete_LIMIT(string directoryPath, int limit);
    }
}
