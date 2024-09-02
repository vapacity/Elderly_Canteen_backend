using Elderly_Canteen.Data.Dtos.Category;
using Elderly_Canteen.Filter;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elderly_Canteen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRole("admin")]
    public class CategoryController : ControllerBase
    {
        private readonly ICateService _cateService;

        public CategoryController(ICateService cateService)
        {
            _cateService = cateService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetAllCategories(string? name)
        {
            var response = await _cateService.GetCate(name);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddCategory(CateRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new CateResponseDto
                {
                    Message = "未接受到数据",
                    Success = false,
                });
            }
            var response = await _cateService.AddCate(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateCategory([FromBody] CateRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new CateResponseDto
                {
                    Message = "未接受到数据",
                    Success = false,
                });
            }
            var response = await _cateService.UpdateCate(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteCategory(string id)
        {
            var response = await _cateService.DeleteCate(id);
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
