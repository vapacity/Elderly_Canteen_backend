using Elderly_Canteen.Data.Dtos.Order;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IOrderService
    {
        Task<MenuResponseDto> GetMenuToday();
    }
}
