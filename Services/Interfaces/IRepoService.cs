using Elderly_Canteen.Data.Dtos.Ingredient;
using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IRepoService
    {
        Task<AllRepoResponseDto> GetRepo();
        //Task<RepoResponseDto> AddRepo(RepoRequestDto dto);
        Task<RepoResponseDto> UpdateRepo(RepoRequestDto dto);
        Task<RepoResponseDto?> DeleteRepo(string ingreId,DateTime expiry);
        Task<RestockResponseDto> Restock(RestockRequestDto dto,string adminId);
        Task<List<RestockResponseDto>> GetRestockHistory();

    }
}
