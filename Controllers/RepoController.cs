using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elderly_Canteen.Controllers
{
    [Route("api/ingredients")]
    [ApiController]
    public class RepoController : ControllerBase
    {
        private readonly IRepoService _repoService;

        public RepoController(IRepoService repoService)
        {
            _repoService = repoService;
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddIngredient(IngreRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new RepoResponseDto
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

        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateIngredient([FromBody]IngreRequestDto dto,string id)
        {
            if (dto == null)
            {
                return BadRequest(new RepoResponseDto
                {
                    message = "未接受到数据",
                    success = false,
                });
            }
            var response = await _repoService.UpdateIngredient(dto,id);
            if (response.success == false)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPut("delete/{id}")]
        public async Task<ActionResult> DeleteIngredient(string id)
        {
            var response = await _repoService.DeleteIngredient(id);
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
