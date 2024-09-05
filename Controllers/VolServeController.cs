using Elderly_Canteen.Data.Dtos.VolServe;
using Elderly_Canteen.Filter;
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Xml.Linq;

namespace Elderly_Canteen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRole("volunteer")]
    public class VolServeController : ControllerBase
    {
        private readonly IVolServeService _volServeService;

        public VolServeController(IVolServeService volServeService)
        {
            _volServeService = volServeService;
        }


        [HttpGet("getAcceptableOrder")]
        public async Task<IActionResult> GetAccessOrders()
        {
            var response = await _volServeService.GetAccessOrder();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("postAcceptOrder")]
        public async Task<IActionResult> AcceptOrder([FromBody]AcceptRequestDto dto)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _volServeService.AcceptOrderAsync(dto.OrderId,accountId);
            if (!response.success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpPost("postConfirmDelivered")]
        public async Task<IActionResult> ConfirmDelivered([FromBody] AcceptRequestDto dto)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _volServeService.ConfirmDeliveredAsync(dto.OrderId,accountId);
            if (!response.success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("getAcceptedOrder")] 
        public async Task<IActionResult> GetAcceptedOrder()
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _volServeService.GetAcceptedOrder(accountId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("getFinishedOrder")]
        public async Task<IActionResult> GetFinishedOrder()
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _volServeService.GetFinishedOrder(accountId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}
