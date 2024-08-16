using Elderly_Canteen.Data.Dtos.Cart;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> CreateCartAsync(string accountId);
    }
}
