
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Elderly_Canteen.Data.Dtos.Volunteer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
    }
}
