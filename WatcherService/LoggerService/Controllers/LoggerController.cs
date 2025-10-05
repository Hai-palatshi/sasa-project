using LoggerService.Models;
using LoggerService.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoggerService.Controllers
{
    [Route("api/logger")]
    [ApiController]
    [Authorize]
    public class LoggerController : ControllerBase
    {
        private readonly ILogFileCreator logFileCreator;
        ILogger<LoggerController> log;

        public LoggerController(ILogFileCreator _logFileCreator, ILogger<LoggerController> _log)
        {
            logFileCreator = _logFileCreator;
            log = _log;
        }

        [HttpPost]
        [Route("log")]
        public async Task<IActionResult> LoggerServices([FromBody] FileMetadata fileMetadata)
        {
            if (!ModelState.IsValid)
            {
                log.LogWarning($"Invalid payload: {ModelState}");
                return BadRequest(ModelState);
            }

            try
            {
                await logFileCreator.CreateLogFileAsync(fileMetadata);
                log.LogInformation("Log TXT created for {File}", fileMetadata.filename);
            }
            catch
            {
                log.LogError("IO error while creating log for {File}", fileMetadata.filename);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }
    }
}
