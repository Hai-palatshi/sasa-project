using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatcherService.Models.DTO;
using WatcherService.Services;
using WatcherService.Services.IServices;

namespace WatcherService.Controllers
{
    [Route("api/config")]
    [ApiController]

    public class ConfigController : ControllerBase
    {
        private readonly IConfigService configService;
        ILogger<ConfigController> log;
        public ConfigController(ILogger<ConfigController> _log, IConfigService _configService)
        {
            configService = _configService;
            log = _log;
        }


        [HttpPost]
        public async Task<IActionResult> UpdateConfiguration([FromBody] UpdateWatcherSettingsRequestDto newSettings)
        {
            if (!ModelState.IsValid)
            {
                log.LogError($"Invalid model state for UpdateConfiguration: {ModelState}");
                return ValidationProblem(ModelState);
            }

            var saved = await configService.UpdateAsync(newSettings);
            log.LogInformation("Configuration updated successfully.");  
            return Ok(new { message = "Configuration updated.", saved });
        }
    }
}
