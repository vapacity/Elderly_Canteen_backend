using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Dtos.Admin;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Permissions;

namespace Elderly_Canteen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IAccountService _accountService;

        public AdminController(IAdminService adminService, IAccountService accountService)
        {
            _adminService = adminService;
            _accountService = accountService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminResponseDto>> GetAdminById(string id)
        {
            var admin = await _adminService.GetAdminByIdAsync(id);
            if (admin == null || !admin.Success)
            {
                return NotFound(admin);
            }
            return Ok(admin);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdmin(string id, AdminRequestDto admin)
        {
            try
            {
                await _adminService.UpdateAdminAsync(id, admin);

                return Ok(new { success = true, msg = "修改成功" });
            }
            catch (InvalidOperationException ex)
            {
                // 如果不存在，返回 404 Not Found
                return NotFound(new { success = false, msg = ex.Message });
            }
            catch (Exception ex)
            {
                // 处理其他可能的错误，返回 500 Internal Server Error
                return StatusCode(500, new { success = false, msg = $"内部服务器错误: {ex.Message}" });
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddAdmin(AdminRegisterDto admin)
        {
            try
            {
                await _adminService.AddAdminAsync(admin);
                return Ok(new { success = true, msg = "创建成功!初始密码为1" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, msg = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, msg = $"内部服务器错误: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdmin(string id)
        {
            try
            {
                await _adminService.DeleteAdminAsync(id);
                return Ok(new { success = true, msg = "删除成功!" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, msg = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, msg = $"内部服务器错误: {ex.Message}" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<AdminSearchDto>> SearchAdmin([FromQuery] string? name, [FromQuery] string? identity)
        {
            var res = await _adminService.SerchAdminAsync(name, identity);
            if (res == null || !res.Success)
            {
                return NotFound(res);
            }
            return Ok(res);
        }
    }
}

