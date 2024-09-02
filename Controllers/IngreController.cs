using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using Elderly_Canteen.Data.Dtos.Ingredient;
using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Filter;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elderly_Canteen.Controllers
{
    [Route("api/ingredients")]
    [ApiController]
    [AuthorizeRole("admin")]
    public class IngreController : ControllerBase
    {
        private readonly IIngreService _ingreService;

        public IngreController(IIngreService ingreService)
        {
            _ingreService = ingreService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetAllIngredient(string? name)
        {
            var response = await _ingreService.GetRepo(name);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddIngredient(IngreRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new IngreResponseDto
                {
                    message = "未接受到数据",
                    success = false,
                });
            }
            var response = await _ingreService.AddIngredient(dto);
            if (response.success == false)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }

        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateIngredient([FromBody]IngreRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new IngreResponseDto
                {
                    message = "未接受到数据",
                    success = false,
                });
            }
            var response = await _ingreService.UpdateIngredient(dto);
            if (response.success == false)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteIngredient(string id)
        {
            var response = await _ingreService.DeleteIngredient(id);
            if (response.success == false)
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
