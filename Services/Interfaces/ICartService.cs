using Elderly_Canteen.Data.Dtos.Cart;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> CreateCartAsync(string accountId);
        Task<CartResponseDto> DeleteCartAsync(string accountId);
        Task<CartItemResponseDto> UpdateCartItemAsync(CartItemRequestDto dto,string accountId);
        Task<CartItemResponseDto> DeleteCartItem(DeleteRequestDto dto, string accountId);
        //Task<CartItemResponseDto> EnsureCartItem(string cartId, string accountId);
    }
}
