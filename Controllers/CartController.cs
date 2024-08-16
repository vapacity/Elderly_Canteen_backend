using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elderly_Canteen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController:ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        [HttpPost("createCart")]
        public async Task<IActionResult> CreateCart()
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cartResponse = await _cartService.CreateCartAsync(accountId);
            if (cartResponse == null)
            {
                return NotFound();
            }
            if (cartResponse.success == false)
            {
                return BadRequest(cartResponse);
            }
            return Ok(cartResponse);
        }
    }
}
