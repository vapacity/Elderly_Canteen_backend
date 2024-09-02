using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Dtos.Users;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Permissions;

namespace Elderly_Canteen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IAccountService _accountService;

        public UsersController(IUsersService usersService, IAccountService accountService)
        {
            _usersService = usersService;
            _accountService = accountService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsersResponseDto>> GetUserById(string id)
        {
            var user = await _usersService.GetUserByIdAsync(id);
            if (user == null || !user.Success)
            {
                return NotFound(user);
            }
            return Ok(user);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UsersRequestDto user)
        {
            try
            {
                await _usersService.UpdateUserAsync(id, user);

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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            try
            {
                await _usersService.DeleteUserAsync(id);
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

        [HttpPost("resetpsd/{id}")]
        public async Task<ActionResult> ResetUserPsd(string id)
        {
            var psd = await _usersService.CreatePsdAsync();
            bool result = await _accountService.ChangePassword(psd, id);
            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "重置密码失败"
                });
            }

            return Ok(new
            {
                success = true,
                msg = "重置密码成功",
                password=psd
            });
        }
    }
}

