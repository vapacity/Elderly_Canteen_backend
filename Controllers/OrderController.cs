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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("getMenuToday")]
        public async Task<IActionResult> GetMenuToday()
        {
            var response = await _orderService.GetMenuToday();
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("getPastOrder")]
        public async Task<IActionResult> GetHistory()
        {
            string accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _orderService.GetHistoryOrderInfoAsync(accountId);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
            
        }
    }
}
