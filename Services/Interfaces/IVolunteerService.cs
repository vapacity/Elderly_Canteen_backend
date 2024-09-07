using Elderly_Canteen.Data.Dtos.Volunteer;
using Microsoft.AspNetCore.Mvc;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IVolunteerService
    {
        Task ApplyAsync(VolunteerApplicationDto application);

        Task<VolunteerReviewListDto> GetAllApplyAsync();

        Task<VolunteerReviewInfoDto> GetApplyInfoAsync(string applyId);

        Task ReviewApplicationAsync(VolunteerReviewApplicationDto application, string id,string accountId);

        Task<VolunteerResponseDto> GetVolInfoAsync(string accountId);

        Task<VolunteerListDto> GetAllVolunteerAsync();

        Task DelVolunteerAsync(string accountId, string adminId);
    }
}
