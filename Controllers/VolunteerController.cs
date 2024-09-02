
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Elderly_Canteen.Data.Dtos.Volunteer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Elderly_Canteen.Filter;
using Elderly_Canteen.Data.Entities;

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
        [Authorize]
        public async Task<IActionResult> Apply(VolunteerApplicationDto application)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
  
            try
            {
                await _volunteerService.ApplyAsync(application, accountId);

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
        [AuthorizeRole("admin")]
        public async Task<IActionResult> getAllApply()
        {
            var response = await _volunteerService.GetAllApplyAsync();
            return Ok(response);
        }

        [HttpGet("applyInfo/{id}")]
        [AuthorizeRole("admin")]
        public async Task<IActionResult> GetApplyInfo(string id)
        {
            var response = await _volunteerService.GetApplyInfoAsync(id);
            return Ok(response);
        }

        [HttpPost("review/{id}")]
        [AuthorizeRole("admin")]
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
                return Unauthorized(new { msg = "无效的Token" });
            }

            // 调用Service函数获取个人信息
            var result = await _volunteerService.GetVolInfoAsync(accountId);
            if (result.success == false)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
