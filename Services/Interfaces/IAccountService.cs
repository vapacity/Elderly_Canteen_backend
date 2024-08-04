using System.Threading.Tasks;
using Elderly_Canteen.Data.Dtos;
using Elderly_Canteen.Data.Dtos.Login;
using Elderly_Canteen.Data.Dtos.Register;
using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Dtos.Account;
namespace Elderly_Canteen.Services.Interfaces
{
    public interface IAccountService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
        Task<PersonInfoResponseDto> GetPersonInfoAsync(string account_id);
        Task<PersonInfoResponseDto> AlterPersonInfoAsync(PersonInfoRequestDto personInfo, string account_id);
        Task<List<AccountDto>> GetAllAccountsAsync();
    }
}
