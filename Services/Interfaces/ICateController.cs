using Elderly_Canteen.Data.Dtos.Category;
using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface ICateService
    {
        Task<AllCateResponseDto> GetCate(string? name);
        Task<CateResponseDto> AddCate(CateRequestDto dto);
        Task<CateResponseDto> UpdateCate(CateRequestDto dto);
        Task<CateResponseDto?> DeleteCate(string ingreId);


    }
}
