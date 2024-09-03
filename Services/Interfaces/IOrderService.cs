using Elderly_Canteen.Data.Dtos.Order;
using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IOrderService
    {
        Task<MenuResponseDto> GetMenuToday();

        // 创建订单逻辑
        Task<OrderInfoDto> CreateOrderAsync(string cartId, string accountId, bool deliver_or_dining,string financeId,List<CartItem> cartItems);

        Task<decimal> CalculateTotalPrice(List<CartItem> cartItems);
    }
}
