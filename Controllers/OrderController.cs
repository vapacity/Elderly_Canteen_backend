
﻿using Elderly_Canteen.Data.Dtos.Cart;
using Elderly_Canteen.Data.Entities;
﻿using Elderly_Canteen.Data.Dtos.Order;
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpPost("postConfirmOrder")]
        public async Task<IActionResult> ConfirmOrder([FromBody] NormalOrderRequestDto dto)
        {
            string accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _orderService.ConfirmOrderAsync(dto.OrderId, accountId);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("getOrderDeliverMsg")]
        public async Task<IActionResult> GetOrderDeliverMsg([FromQuery] string OrderId)
        {
            var response = await _orderService.GetODMsg(OrderId);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("getOrderMsg")]
        public async Task<IActionResult> GetOrderMsg([FromQuery] string OrderId)
        {
            var response = await _orderService.GetOrderInfoByIdAsync(OrderId);
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("postDiningComment")]
        public async Task<IActionResult> SubmitReview([FromBody] ReviewSubmissionDto review)
        {
            if (review == null || string.IsNullOrWhiteSpace(review.OrderId) || review.CStars < 1 || review.CStars > 5)
            {
                return BadRequest("输入格式有误！");
            }

            if (review.CReviewText?.Length > 150)
            {
                return BadRequest("评价不超过50个字！");
            }

            try
            {
                var result = await _orderService.SubmitDiningReviewAsync(review);
                if (result.success == false)
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, msg = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("getDiningComment")]
        public async Task<IActionResult> GetDiningReview([FromQuery] string OrderId)
        {
            if (string.IsNullOrEmpty(OrderId))
            {
                return BadRequest("需要订单ID");
            }

            try
            {
                var review = await _orderService.GetReviewByOrderId(OrderId);
                if (review.success == false)
                {
                    return NotFound(review);
                }
                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    msg = $"服务器内部错误: {ex.Message}"
                });
            }
        }
        [HttpPost("postDeliverComment")]
        public async Task<IActionResult> SubmitDeliverReview([FromBody] ReviewSubmissionDto review)
        {
            if (review == null || string.IsNullOrWhiteSpace(review.OrderId))
            {
                return BadRequest("需要订单ID.");
            }

            if (review.CStars < 1 || review.CStars > 5)
            {
                return BadRequest("口味评分必须在1到5星之间。");
            }

            // 检查DStars是否存在且在有效范围内
            if (review.DStars.HasValue && (review.DStars < 1 || review.DStars > 5))
            {
                return BadRequest("配送评分必须在1到5星之间。");
            }

            if (review.DReviewText != null && review.CReviewText?.Length > 150)
            {
                return BadRequest("口味评价不应超过50个中文字符。");
            }

            // 检查DReviewText是否存在且长度超标
            if (!string.IsNullOrEmpty(review.DReviewText) && review.DReviewText.Length > 150)
            {
                return BadRequest("配送评价不应超过50个中文字符。");
            }

            try
            {
                var result = await _orderService.SubmitDeliveringReviewAsync(review);
                if (!result.success)
                {
                    return NotFound(new { success = false, msg = result.msg });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, msg = $"内部服务器错误: {ex.Message}" });
            }
        }
        [HttpGet("getDeliverComment")]
        public async Task<IActionResult> GetDeliveringReview([FromQuery] string OrderId)
        {
            if (string.IsNullOrEmpty(OrderId))
            {
                return BadRequest("需要订单ID");
            }

            try
            {
                var review = await _orderService.GetDeliveringReviewByOrderId(OrderId);
                if (review.success == false)
                {
                    return NotFound(review);
                }
                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    msg = $"服务器内部错误: {ex.Message}"
                });
            }
        }

    }
}

