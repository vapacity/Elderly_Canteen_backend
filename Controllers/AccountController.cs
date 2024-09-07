using Elderly_Canteen.Data.Dtos.Login;
using Elderly_Canteen.Data.Dtos.Register;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Dtos.AuthenticationDto;
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Tools;
using Elderly_Canteen.Data.Dtos.OTP;
using System.Collections.Generic;
/*
 * 401
 */
namespace Elderly_Canteen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        [Consumes("application/json")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            var loginResponse = await _accountService.LoginAsync(loginRequest);

            if (!loginResponse.loginSuccess)
            {
                if (loginResponse.msg == "Account does not exist")
                {
                    return NotFound(loginResponse);
                }

                if (loginResponse.msg == "Incorrect password")
                {
                    return BadRequest(loginResponse);
                }
                return NotFound(loginResponse);
            }

            return Ok(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            // 将头像文件传递给Service层进行处理
            var result = await _accountService.RegisterAsync(registerRequestDto);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("getPersonInfo")]
        public async Task<IActionResult> GetPersonInfo()
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountId))
            {
                return Unauthorized(new { msg = "无效的Token" });
            }

            // 调用Service函数获取个人信息
            var result = await _accountService.GetPersonInfoAsync(accountId);
            if (result.getSuccess == false)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("alterPersonInfo")]
        public async Task<IActionResult> AlterPersonInfo([FromForm] PersonInfoRequestDto personInfo,IFormFile? avatar)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountId))
            {
                return Unauthorized(new { msg = "无效的Token" });
            }

            // 将头像文件传递给Service层进行处理
            var response = await _accountService.AlterPersonInfoAsync(personInfo, accountId, avatar);
            if (response.getSuccess == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }


        [HttpGet("getAllAccount")]
        public async Task<IActionResult> GetAllAccount()
        {
            var response = await _accountService.GetAllAccountsAsync();
            return Ok(response);
        }

 
        [HttpPost("nameAuthenticate")]
        public async Task<IActionResult> NameAutenticate(AuthenticationRequestDto input)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _accountService.NameAuthenticationBatchAsync(input);

            return Ok(result);
        }

        //(url参数安全性较低)
        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(string pswd)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool result = await _accountService.ChangePassword(pswd, accountId);
            if (result == false) {
                return BadRequest(new
                {
                    success = false,
                    msg = "修改失败",
                });
            }
            return Ok(new
            {
                success = true,
                msg = "修改成功",
            });
        }

        [HttpPost("sendOTP")]
        public async Task<IActionResult> RequestVerificationCode([FromBody] GetOTPRequestDto request)
        {
            var response = await _accountService.SendOTPAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("verifiationCodeLogin")]
        public async Task<IActionResult> VerifyLoginOTP([FromBody] VerifyOTPRequestDto request)
        {
            var response = await _accountService.VerifyLoginOTPAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("verifiationCodeWithoutUserCheck")]
        public async Task<IActionResult> VerifyOTPWithoutUserCheck([FromBody] VerifyOTPRequestDto request)
        {
            var response = await _accountService.VerifyOTPWithoutUserCheckAsync(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPost("alterPassword")]
        public async Task<IActionResult> AlterPassword([FromBody] PasswordRequestDto request)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 首先验证原密码是否正确
            bool isOldPasswordValid = await _accountService.VerifyPassword(request.OldPassword, accountId);
            if (!isOldPasswordValid)
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "原密码不正确",
                });
            }

            // 如果原密码正确，尝试修改密码
            bool result = await _accountService.ChangePassword(request.NewPassword, accountId);
            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    msg = "修改失败",
                });
            }

            return Ok(new
            {
                success = true,
                msg = "修改成功",
            });
        }

        [Authorize]
        [HttpPost("changePhone")]
        public async Task<IActionResult> ChangePhone([FromBody] PhoneRequestDto request)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _accountService.ChangePhone(request, accountId);
            if (result.success == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (accountId == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    msg = "用户未认证",
                });
            }

            var result = await _accountService.DeleteAccountAsync(accountId);

            if (result)
            {
                // 成功删除用户账号后，可以清理用户会话或Cookie等
                return Ok(new
                {
                    success = true,
                    msg = "账号已成功注销",
                });
            }

            return BadRequest(new
            {
                success = false,
                msg = "注销账号时发生错误",
            });
        }

        [Authorize]
        [HttpPost("prePaid")]
        public async Task<IActionResult> CreditAccount([FromBody] decimal money)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var result = await _accountService.CreditAccount(accountId,money);
                if (result.success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
