﻿using Elderly_Canteen.Data.Dtos.Dish;
namespace Elderly_Canteen.Services.Interfaces
{
    public interface IDishService
    {
        Task<DishResponseDto> AddDish(DishRequestDto dto);
        Task<DishResponseDto> UpdateDish(DishRequestDto dto);
        Task<DishResponseDto> DeleteDish(string dishId);

        Task<string> UploadImageAsync(string id, IFormFile image);

        Task<AllDishResponseDto> SearchDishesAsync(string? name,string? category);
    }
}
