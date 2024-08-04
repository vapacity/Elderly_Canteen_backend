using Elderly_Canteen.Data.Dtos.Login;
using Elderly_Canteen.Data.Dtos.Register;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Entities;

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

            if (!loginResponse.Success)
            {
                if (loginResponse.Msg == "Account does not exist")
                {
                    return NotFound(loginResponse);
                }

                if (loginResponse.Msg == "Incorrect password")
                {
                    return BadRequest(loginResponse);
                }
            }

            return Ok(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            var result = await _accountService.RegisterAsync(registerRequestDto);

            if (result.Msg == "用户已存在")
            {
                return BadRequest(new RegisterResponseDto
                {
                    Msg = result.Msg,
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

            return Ok(new RegisterResponseDto
            {
                Msg = result.Msg
            });
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
    }
}
