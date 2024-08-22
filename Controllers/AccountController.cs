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
            var result = await _accountService.RegisterAsync(registerRequestDto);

            if (result.msg == "用户已存在")
            {
                return BadRequest(result);
            }
            
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
        public async Task<IActionResult> AlterPersonInfo(PersonInfoRequestDto personInfo)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountId))
            {
                return Unauthorized(new { msg = "无效的Token" });
            }
            // 调用Service函数获取个人信息
            var response = await _accountService.AlterPersonInfoAsync(personInfo,accountId);
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

        [Authorize]
        [HttpPost("nameAuthenticate")]
        public async Task<IActionResult> NameAutenticate(AuthenticationRequestDto input)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _accountService.NameAuthentication(input, accountId);
            if (result.success == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
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
    }
}
