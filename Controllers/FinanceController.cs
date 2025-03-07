using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Filter;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elderly_Canteen.Controllers
{
    [ApiController]
    [Authorize]
    [AuthorizeRole("admin")]
    [Route("api/[controller]")]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService  _financeService;

        public FinanceController(IFinanceService financeService)
        {
            _financeService = financeService;
        }
        [HttpGet("financial-records")]
        public async Task<IActionResult> GetFinances([FromQuery] string financeType = null,[FromQuery] string inOrOut = null,[FromQuery] string financeDate = null,[FromQuery] string financeId = null,[FromQuery] string accountId = null,[FromQuery] string status = null)
        {
            // 传递可能为空或默认值的参数
            var result = await _financeService.GetFilteredFinanceInfoAsync(
                financeType: financeType,
                inOrOut: inOrOut,
                financeDate: financeDate,
                financeId: financeId,
                accountId: accountId,
                status:status);

            if (result.success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
        [Authorize]
        [HttpPost("financial-records/{id}/status")]
        public async Task<IActionResult> AuditFinance(string id,[FromBody] AuditFinanceRequest request)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _financeService.UpdateFinanceStatusAsync(id, request.Status, accountId);

            if (result.success)
            {
                return Ok(result);
            }
            return BadRequest(result.msg);
        }
        public class AuditFinanceRequest
        {
            public string Status { get; set; }
        }
        [HttpGet("getTotal")]
        public async Task<IActionResult> GetTotalFinanceInfo()
        {
            var result = await _financeService.GetTotalFinanceInfo();
            if (!result.success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
   
}
