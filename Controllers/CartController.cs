using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Elderly_Canteen.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController:ControllerBase
    {
        private readonly ICartService _cartService;
        
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string accountId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpPost("createCart")]
        public async Task<IActionResult> CreateCart()
        {
            
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

        [HttpDelete("deleteCart")]
        public async Task<IActionResult> DeleteCart()
        {
            var cartResponse = await _cartService.DeleteCartAsync(accountId);
            if (cartResponse == null)
            {
                return NotFound();
            }
            if (!cartResponse.success)
            {
                return BadRequest(cartResponse);
            }
            return Ok(cartResponse);
        }

        [HttpPost("updateCart")]
        public async Task<IActionResult> UpdateCart()
        {
            return null;
        }

        [HttpDelete("deleteCartItem")]
        public async Task<IActionResult> DeleteCartItem()
        {
            return null;
        }

        [HttpPost("ensureCart")]
        public async Task<IActionResult> EnsureCart()
        {
            return null;
        }
    }
}
