using Elderly_Canteen.Data.Dtos.Category;
using Elderly_Canteen.Data.Dtos.Dish;
using Elderly_Canteen.Filter;
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elderly_Canteen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRole("admin")]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchDishes(string? name,string? category)
        {
            var response = await _dishService.SearchDishesAsync(name,category);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPost("addDish")]
        public async Task<ActionResult> AddDish(DishRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new DishResponseDto
                {
                    Msg = "未接受到数据",
                    Success = false,
                    Dish = null
                });
            }
            var response = await _dishService.AddDish(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPut("updateDish")]
        public async Task<ActionResult> UpdateCategory(DishRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new DishResponseDto
                {
                    Msg = "未接受到数据",
                    Success = false,
                    Dish = null
                });
            }
            var response = await _dishService.UpdateDish(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpDelete("delete/{dishId}")]
        public async Task<ActionResult> DeleteDish(string dishId)
        {
            if (dishId == null)
            {
                return BadRequest(new DishResponseDto
                {
                    Msg = "未接受到数据",
                    Success = false,
                    Dish = null
                });
            }
            var response = await _dishService.DeleteDish(dishId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage(string DishId, IFormFile image)
        {
            try
            {
                string url = await _dishService.UploadImageAsync(DishId, image);

                return Ok(new { Success = true, Msg = "图片上传成功" ,imageUrl = url});
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Success = false, Msg = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Msg = $"上传失败: {ex.Message}" });
            }
        }

    }
}
