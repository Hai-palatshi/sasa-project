using WatcherService.Models;
using WatcherService.ResponseAPI_Http;

namespace WatcherService.Services.IServices
{
    public interface IApiManageService
    {
        public Task<APIResponse> PostData(FileMetadata data,string token);
    }
}
