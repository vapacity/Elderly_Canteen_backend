using Elderly_Canteen.Data.Dtos.Cart;
using System.Threading.Tasks;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> CreateCartAsync(string accountId);
        Task<CartResponseDto> DeleteCartAsync(string accountId);
        Task<CartItemResponseDto> UpdateCartItemAsync(CartItemRequestDto dto,string accountId);
        Task<CartItemResponseDto> DeleteCartItem(DeleteRequestDto dto, string accountId);
        Task<CartItemResponseDto> EnsureCartItem(string cartId, bool? set_default_add,string? newAddress,bool deliver_or_dining,string? remark, string accountId);
        Task<CartItemsDto> GetCartItemsAsync(string cartId, string accountId);
        Task DeleteUnassociatedCartsAsync();
        Task<bool> ClearItemsAsync(string cartId);
    }
}
