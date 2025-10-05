using WatcherService.Models;
using WatcherService.Models.DTO;

namespace WatcherService.Services.IServices
{
    public interface IConfigService
    {
        Task<UpdateWatcherSettingsRequest> UpdateAsync(UpdateWatcherSettingsRequestDto dto);
    }
}
