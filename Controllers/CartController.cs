using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Elderly_Canteen.Data.Dtos.Cart;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
namespace Elderly_Canteen.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

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

        [HttpDelete("deleteCart")]
        public async Task<IActionResult> DeleteCart()
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        [HttpPost("updateCartItem")]
        public async Task<IActionResult> UpdateCart(CartItemRequestDto dto)
        {
            // 获取当前用户的 AccountId
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                // 如果 accountId 为空，返回未经授权的响应
                return Unauthorized(new { Success = false, Message = "用户未授权" });
            }

            // 调用服务层的方法来更新购物车项
            var response = await _cartService.UpdateCartItemAsync(dto, accountId);

            // 根据响应结果返回相应的 IActionResult
            if (response.Success)
            {
                return Ok(new { Success = true, Message = response.Message });
            }
            else
            {
                return BadRequest(new { Success = false, Message = response.Message });
            }
        }


        [HttpDelete("deleteCartItem")]
        public async Task<IActionResult> DeleteCartItem([FromBody] DeleteRequestDto dto)
        {
            // 获取当前用户的 AccountId
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                return Unauthorized(new { Success = false, Message = "用户未授权" });
            }

            // 调用服务层的方法来删除购物车项
            var response = await _cartService.DeleteCartItem(dto, accountId);

            // 根据服务层返回的结果返回相应的 IActionResult
            if (response.Success)
            {
                return Ok(new { Success = true, Message = response.Message });
            }
            else
            {
                return BadRequest(new { Success = false, Message = response.Message });
            }
        }


        [HttpPost("ensureCart")]
        public async Task<IActionResult> EnsureCart([FromBody] string CART_ID, string? newAddress,bool deliver_or_dining)
        {
            // 获取当前用户的 AccountId
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string cartId = CART_ID;
            var response = await _cartService.EnsureCartItem(CART_ID, newAddress,deliver_or_dining, accountId);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("getCartItem{cartId}")]
        public async Task<IActionResult> GetCartItems(string cartId)
        {
            // 获取当前用户的 AccountId
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _cartService.GetCartItemsAsync(cartId, accountId);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPost("clearCart")]
        public async Task<IActionResult> ClearCart([FromBody]NormalRequestDto dto) 
        {
            // 获取当前用户的 AccountId
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _cartService.ClearItemsAsync(dto.cartId);
            if (response)
            {
                return Ok(new
                {
                    success = true,
                    msg = "successfully clear"
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "cartId doesn't exist or has been associated with order"
                });
            }
        }
    }
}
