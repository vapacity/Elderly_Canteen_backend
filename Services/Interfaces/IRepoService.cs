using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IRepoService
    {
        Task<RepoResponseDto> GetRepo(string ingreId);
        Task<RepoResponseDto> AddIngredient(IngreRequestDto dto);
        Task<RepoResponseDto> UpdateIngredient(IngreRequestDto dto,string id);
        Task<RepoResponseDto?> DeleteIngredient(string ingreId,DateTime expiry);
        Task<RepoResponseDto> Restock(string ingreId);

    }
}
