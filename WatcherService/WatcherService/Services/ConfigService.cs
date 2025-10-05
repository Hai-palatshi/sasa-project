using AutoMapper;
using System.Text.Json;
using System.Text;
using WatcherService.Models;
using WatcherService.Models.DTO;
using WatcherService.Services.IServices;
using Newtonsoft.Json;

namespace WatcherService.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment env;

        public ConfigService(IMapper _mapper, IWebHostEnvironment _env)
        {
            mapper = _mapper;
            env = _env;

        }
        public async Task<UpdateWatcherSettingsRequest> UpdateAsync(UpdateWatcherSettingsRequestDto dto)
        {
            var SettingsRequest = mapper.Map<UpdateWatcherSettingsRequest>(dto);

            var filePath = Path.Combine(env.ContentRootPath, "configFile", "WatcherSettings.json");

            var json = JsonConvert.SerializeObject(SettingsRequest, Formatting.Indented);

            await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);

            return SettingsRequest;

        }
    }
}
