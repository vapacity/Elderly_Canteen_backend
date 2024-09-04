using Elderly_Canteen.Data.Dtos.Ingredient;
using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IRepoService
    {
        Task<AllRepoResponseDto> GetRepo(string? name);
        //Task<RepoResponseDto> AddRepo(RepoRequestDto dto);
        Task<RepoResponseDto> UpdateRepo(RepoRequestDto dto);
        Task<RepoResponseDto?> DeleteRepo(string ingreId,DateTime expiry);
        Task<RestockResponseDto> Restock(RestockRequestDto dto,string adminId);
        Task<AllRestockResponseDto> GetRestockHistory();

        // 检查并减少库存
        Task<bool> CheckAndReduceStockAsync(string dishId, DateTime week, int quantity);
        Task CheckAndRemoveExpiredIngredientsAsync();
        // 每日自动库存补货
        Task ReplenishDailyStockAsync();

        // 减少食材数量
        // 输入dishId 和 减少数量可以直接减少食材
        Task<bool> ReduceIngredientStockAsync(string dishId, int requiredQuantity);

        // 计算当前能制作的dish的最大份数
        Task<int> CalculateMaxPortionsAsync(string dishId, int requestedQuantity = 50);
    }
}
