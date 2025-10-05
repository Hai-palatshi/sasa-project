using LoggerService.Models;

namespace LoggerService.Services.IServices
{
    public interface ILogFileCreator
    {
        Task CreateLogFileAsync(FileMetadata metadata);
    }
}
