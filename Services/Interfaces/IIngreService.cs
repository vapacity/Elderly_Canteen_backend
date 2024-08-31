using Elderly_Canteen.Data.Dtos.Ingredient;
using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IIngreService
    {
        Task<AllIngreResponseDto> GetRepo();
        Task<IngreResponseDto> AddIngredient(IngreRequestDto dto);
        Task<IngreResponseDto> UpdateIngredient(IngreRequestDto dto);
        Task<IngreResponseDto?> DeleteIngredient(string ingreId);


    }
}
