using Elderly_Canteen.Data.Dtos.Volunteer;
using Microsoft.AspNetCore.Mvc;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IVolunteerService
    {
        Task ApplyAsync(VolunteerApplicationDto application, string accountId);
    }
}
