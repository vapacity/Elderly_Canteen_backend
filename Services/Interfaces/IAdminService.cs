using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using Elderly_Canteen.Data.Dtos.Admin;
using System.Security.Principal;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminResponseDto> GetAdminByIdAsync(string id);
        Task UpdateAdminAsync(string id, AdminRequestDto admin);
        Task AddAdminAsync(AdminRegisterDto registerRequestDto);
        Task DeleteAdminAsync(string id);
        Task<AdminSearchDto> SerchAdminAsync(string? accountName, string? identity);
    }
}
