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
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto, IFormFile avatar);
        Task<PersonInfoResponseDto> GetPersonInfoAsync(string account_id);
        Task<PersonInfoResponseDto> AlterPersonInfoAsync(PersonInfoRequestDto personInfo, string accountId, IFormFile avatar);
        Task<List<AccountDto>> GetAllAccountsAsync();
        Task<AuthenticationResponseDto> NameAuthentication(AuthenticationRequestDto input, string accountId);
        Task<bool> VerifyPassword(string oldPassword, string accountId);
        Task<bool> ChangePassword(string password, string accountId);
        Task<VerifyOTPResponseDto<OTPLoginResponseDto>> VerifyLoginOTPAsync(VerifyOTPRequestDto request);
        Task<VerifyOTPResponseDto<OTPLoginResponseDto>> VerifyOTPWithoutUserCheckAsync(VerifyOTPRequestDto request);
        Task<GetOTPResponseDto> SendOTPAsync(GetOTPRequestDto request);
        Task<PhoneResponseDto> ChangePhone(PhoneRequestDto request, string accountId);
        Task<bool> DeleteAccountAsync(string accountId);
    }
}
