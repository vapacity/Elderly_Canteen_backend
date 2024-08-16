using System.Threading.Tasks;
using Elderly_Canteen.Data.Dtos;
using Elderly_Canteen.Data.Dtos.Login;
using Elderly_Canteen.Data.Dtos.Register;
using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Dtos.Account;
using Elderly_Canteen.Data.Dtos.AuthenticationDto;
using Elderly_Canteen.Data.Dtos.Donate;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IDonateService
    {
        Task<DonationListDto> GetDonateListAsync();

        Task<DonateResponseDto> SubmitDonationAsync(DonateRequestDto request);
    }
}
