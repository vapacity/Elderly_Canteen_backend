using Elderly_Canteen.Data.Dtos.Dish;
namespace Elderly_Canteen.Services.Interfaces
{
    public interface IDishService
    {
        Task<DishResponseDto> AddDish(DishRequestDto dto);

    }
}
