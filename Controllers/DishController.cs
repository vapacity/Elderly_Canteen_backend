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

        [HttpPost("addDish")]
        public async Task<ActionResult> AddCategory(DishRequestDto dto)
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

        [HttpPost("updateDish")]
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
    }
}
