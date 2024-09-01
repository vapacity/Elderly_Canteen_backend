using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using Elderly_Canteen.Data.Dtos.Ingredient;
using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace Elderly_Canteen.Controllers
{
    [Route("api/repo")]
    [ApiController]
    public class RepoController : ControllerBase
    {
        private readonly IRepoService _repoService;

        public RepoController(IRepoService repoService)
        {
            _repoService = repoService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetAllIngredient(string? name)
        {
            var identity = User.FindFirst(ClaimTypes.Role)?.Value;
            if (identity != "admin")
                return BadRequest(new RestockResponseDto
                {
                    Success = false,
                    Message = "you are not administrator",
                    Data = null,
                });
            var response = await _repoService.GetRepo(name);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        /*[HttpPost("add")]
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
            var response = await _repoService.AddIngredient(dto);
            if (response.success == false)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }

        }
*/
        [HttpPut("update")]
        public async Task<ActionResult> UpdateRepo(RepoRequestDto dto)
        {
            var identity = User.FindFirst(ClaimTypes.Role)?.Value;
            if (identity != "admin")
                return BadRequest(new RestockResponseDto
                {
                    Success = false,
                    Message = "you are not administrator",
                    Data = null,
                });
            if (dto == null)
            {
                return BadRequest(new IngreResponseDto
                {
                    message = "未接受到数据",
                    success = false,
                });
            }
            var response = await _repoService.UpdateRepo(dto);
            if (response.success == false)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpDelete("delete/{id}/{expiry}")]
        public async Task<ActionResult> DeleteRepo(string id, DateTime expiry)
        {
            var identity = User.FindFirst(ClaimTypes.Role)?.Value;
            if (identity != "admin")
                return BadRequest(new RestockResponseDto
                {
                    Success = false,
                    Message = "you are not administrator",
                    Data = null,
                });
            var response = await _repoService.DeleteRepo(id, expiry);
            if (response.success == false)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPost("restock")]
        public async Task<ActionResult> RestockStuff(RestockRequestDto dto)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var identity = User.FindFirst(ClaimTypes.Role)?.Value;
            if (adminId == null)
                return BadRequest(new RestockResponseDto
                {
                    Success = false,
                    Message = "not authoriented",
                    Data = null,
                });
            if(identity != "admin")
                return BadRequest(new RestockResponseDto
                {
                    Success = false,
                    Message = "you are not administrator",
                    Data = null,
                });
            var response = await _repoService.Restock(dto, adminId);
            if(response.Success == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("search/restock")]
        public async Task<ActionResult> GetRestockHistory()
        {
            var identity = User.FindFirst(ClaimTypes.Role)?.Value;
            if (identity != "admin")
                return BadRequest(new RestockResponseDto
                {
                    Success = false,
                    Message = "you are not administrator",
                    Data = null,
                });
            var response = await _repoService.GetRestockHistory();
            if(response.Success != false)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
