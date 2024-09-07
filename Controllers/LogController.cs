using Elderly_Canteen.Services.Implements;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Elderly_Canteen.Services.Interfaces;
namespace Elderly_Canteen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _logService.GetLogsAsync();
            return Ok(logs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] LogDto logDto)
        {
            if (logDto == null || string.IsNullOrEmpty(logDto.LogLevel) || string.IsNullOrEmpty(logDto.Message))
            {
                return BadRequest("Invalid log data");
            }

            await _logService.LogAsync(logDto.LogLevel, logDto.Message);
            return Ok();
        }
    }
    public class LogDto
    {
        public string LogLevel { get; set; }
        public string Message { get; set; }
    }
}
