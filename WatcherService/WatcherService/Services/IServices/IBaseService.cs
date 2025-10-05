using WatcherService.RequestAPI_Http;
using WatcherService.ResponseAPI_Http;

namespace WatcherService.Services.IServices
{
    public interface IBaseService
    {
        public Task<APIResponse> SendAsync(RequestDetails apiRequest);

    }
}
