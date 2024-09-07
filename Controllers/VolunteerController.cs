
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Elderly_Canteen.Data.Dtos.Volunteer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Elderly_Canteen.Filter;
using Elderly_Canteen.Data.Entities;
using static System.Net.Mime.MediaTypeNames;

namespace Elderly_Canteen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteerController : ControllerBase
    {
        private readonly IVolunteerService _volunteerService;

        public VolunteerController(IVolunteerService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply(VolunteerApplicationDto application)
        {
            try
            {
                await _volunteerService.ApplyAsync(application);

                return Ok(new { success = true, msg = "申请成功" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, msg = ex.Message });
            }
            catch 
            {
                return BadRequest( new { success = false, msg = "申请失败" });
            }
        }

        [HttpGet("getAllApply")]
        public async Task<IActionResult> getAllApply()
        {
            var response = await _volunteerService.GetAllApplyAsync();
            return Ok(response);
        }

        [HttpGet("applyInfo/{id}")]
        public async Task<IActionResult> GetApplyInfo(string id)
        {
            var response = await _volunteerService.GetApplyInfoAsync(id);
            return Ok(response);
        }

        [HttpPost("review/{id}")]
        [Authorize]
        public async Task<IActionResult> ReviewApplication(VolunteerReviewApplicationDto application,string id)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (accountId == null)
            {
                return Unauthorized(new { success = false, msg = "用户认证失败" });
            }

            try
            {
                await _volunteerService.ReviewApplicationAsync(application, id, accountId);

                return Ok(new { success = true, msg = "审核完成" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, msg = ex.Message });
            }
            catch
            {
                return BadRequest(new { success = false, msg = "审核失败" });
            }
        }

        [Authorize]
        [HttpGet("getVolInfo")]
        public async Task<IActionResult> GetVolInfo()
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountId))
            {
                return Unauthorized(new { success = false, msg = "无效的Token" });
            }

            // 调用Service函数获取个人信息
            var result = await _volunteerService.GetVolInfoAsync(accountId);
            if (result.success == false)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> getAllVolunteer()
        {
            var response = await _volunteerService.GetAllVolunteerAsync();
            return Ok(response);
        }

        [HttpDelete("del/{id}")]
        [Authorize]
        public async Task<IActionResult> delVolunteer(string id)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (accountId == null)
            {
                return Unauthorized(new { success = false, msg = "用户认证失败" });
            }
            try
            {
                await _volunteerService.DelVolunteerAsync(id,accountId);

                return Ok(new { success = true, msg = "删除成功" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, msg = ex.Message });
            }
            catch
            {
                return BadRequest(new { success = false, msg = "删除失败" });
            }
        }

    }
}
