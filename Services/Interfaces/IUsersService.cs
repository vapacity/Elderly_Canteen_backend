using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using Elderly_Canteen.Data.Dtos.Users;
using System.Security.Principal;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IUsersService
    {
        Task<UsersResponseDto> GetUserByIdAsync(string id);
        Task UpdateUserAsync(string id, UsersRequestDto user);
        Task DeleteUserAsync(string id);
        Task<string> CreatePsdAsync(int length = 12, int maxLength = 16);

        Task<UserSearchDto> SerchUserAsync(string? accountName, string? identity);
    }
}
