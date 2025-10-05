using WatcherService.Models;
using WatcherService.RequestAPI_Http;
using WatcherService.ResponseAPI_Http;
using WatcherService.Services.IServices;

namespace WatcherService.Services
{
    public class ApiManageService: IApiManageService
    {
        private readonly IBaseService baseService;
        private readonly IConfiguration configuration;

        public ApiManageService(IBaseService _baseService, IConfiguration _configuration)
        {
            baseService = _baseService;
            configuration = _configuration;
        }


        public async Task<APIResponse> PostData(FileMetadata data,string token)
        {
            return await baseService.SendAsync(new RequestDetails
            {
                Method = RequestDetails.ApiType.POST,
                Data = data,
                Url = configuration["LoggerUrl"],
                token = token
            });
        }
    }
}
