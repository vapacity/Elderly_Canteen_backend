using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Elderly_Canteen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomePageController : ControllerBase
    {
        private readonly IHomePageService _homePageService;

        public HomePageController(IHomePageService homePageService)
        {
            _homePageService = homePageService;
        }

        [HttpGet("dishes")]
        public async Task<IActionResult> GetDishes()
        {
            try
            {
                var dishes = await _homePageService.GetDishesAsync();
                if (dishes == null || !dishes.Any())
                {
                    return NotFound("没有找到菜品");
                }
                return Ok(new
                {
                    success = true,
                    msg = "成功获取菜品列表",
                    response = dishes
                });
            }
            catch (Exception ex)
            {
                // 错误处理
                return StatusCode(500, new
                {
                    success = false,
                    msg = $"获取菜品列表出错: {ex.Message}",
                    response = new List<object>()
                });
            }
        }
        [HttpGet("getWeekmenuAndWeekDiscount")]
        public async Task<ActionResult<List<object>>> GetThisWeekMenu()
        {
            try
            {
                var menu = await _homePageService.GetThisWeekMenu();
                if (menu == null || !menu.Any())
                {
                    return NotFound("没有找到本周的菜单");
                }

                return Ok(new
                {
                    success = true,
                    msg = "成功获取菜品列表",
                    response = menu
                });
            }
            catch(Exception ex)
            {
                // 错误处理
                return StatusCode(500, new
                {
                    success = false,
                    msg = $"获取菜品列表出错: {ex.Message}",
                    response = new List<object>()
                });
            }
        }

        [HttpGet("getDayDiscount")]
        public async Task<ActionResult<List<object>>> GetThisDayDiscount()
        {
            try
            {
                var menu = await _homePageService.GetThisDayDiscountMenu();

                if (!menu.success)
                {
                    return NotFound(menu);
                }
                return Ok(menu);
            }
            catch (Exception ex)
            {
                // 错误处理
                return StatusCode(500, new
                {
                    success = false,
                    msg = $"获取菜品列表出错: {ex.Message}",
                    response = new List<object>()
                });
            }
        }
        [HttpGet("getReviews")]
        public async Task<IActionResult> GetReviews()
        {
            
            try
            {
                var result = await _homePageService.GetReviewsAsync();
                if(result==null)
                {
                    return NotFound("没有找到评价");
                }
                    return Ok(result);
            }
            catch (Exception ex)
            {
                // 错误处理
                return StatusCode(500, new
                {
                    success = false,
                    msg = $"获取评价列表出错: {ex.Message}",
                    response = new List<object>()
                });
            }
        }
    }

}
