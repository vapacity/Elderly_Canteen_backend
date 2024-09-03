using System.Threading.Tasks;
using Elderly_Canteen.Data.Dtos;
using Elderly_Canteen.Data.Dtos.Login;
using Elderly_Canteen.Data.Dtos.Register;
using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Dtos.Account;
using Elderly_Canteen.Data.Dtos.AuthenticationDto;
using Elderly_Canteen.Data.Dtos.OTP;
namespace Elderly_Canteen.Services.Interfaces
{
    public interface IAccountService
    {
        //登录
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        
        //注册
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto, IFormFile avatar);
        
        //获得个人信息
        Task<PersonInfoResponseDto> GetPersonInfoAsync(string account_id);
        
        //修改个人信息
        Task<PersonInfoResponseDto> AlterPersonInfoAsync(PersonInfoRequestDto personInfo, string accountId, IFormFile avatar);
        
        //获得所有人的信息（调试用）
        Task<List<AccountDto>> GetAllAccountsAsync();
        
        //实名认证
        Task<AuthenticationResponseDto> NameAuthentication(AuthenticationRequestDto input, string accountId);
        
        //改密码
        Task<bool> VerifyPassword(string oldPassword, string accountId);
        
        
        Task<bool> ChangePassword(string password, string accountId);
        Task<VerifyOTPResponseDto<OTPLoginResponseDto>> VerifyLoginOTPAsync(VerifyOTPRequestDto request);
        Task<VerifyOTPResponseDto<OTPLoginResponseDto>> VerifyOTPWithoutUserCheckAsync(VerifyOTPRequestDto request);
        Task<GetOTPResponseDto> SendOTPAsync(GetOTPRequestDto request);
        Task<PhoneResponseDto> ChangePhone(PhoneRequestDto request, string accountId);
        Task<bool> DeleteAccountAsync(string accountId);
    }
}
