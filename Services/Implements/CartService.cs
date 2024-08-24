using Elderly_Canteen.Data.Dtos.Cart;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;

namespace Elderly_Canteen.Services.Implements
{
    public class CartService:ICartService
    {
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly IGenericRepository<CartItem> _cartItemRepository;

        public CartService(IGenericRepository<Cart> cartRepository, IGenericRepository<CartItem> cartItemRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        private string GenerateCartId()
        {
            // 获取 Guid 的前 8 个字符
            string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);

            // 获取当前时间戳
            string timestampPart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("X").Substring(0, 2);

            // 合并并确保长度为 10
            return guidPart + timestampPart;
        }
        public DateTime ConvertToBeijingTime(DateTime utcTime)
        {
            // 获取中国标准时间时区信息
            TimeZoneInfo chinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

            // 将 UTC 时间转换为中国标准时间
            DateTime beijingTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, chinaTimeZone);

            return beijingTime;
        }
        public async Task<CartResponseDto> CreateCartAsync(string accountId)
        {
            try
            {
                var existedCart = (await _cartRepository.FindByConditionAsync(cart => cart.AccountId == accountId)).FirstOrDefault();
                if (existedCart != null)
                {
                    // 返回 CartResponseDto
                    return new CartResponseDto
                    {
                        success = true,
                        msg = "Cart already existed!",
                        response = new CartResponseDto.CartResponse
                        {
                            cartId = existedCart.CartId,
                            createTime = existedCart.CreatedTime,
                            updateTime = existedCart.UpdatedTime
                        }
                    };
                }
                // 生成 CartId
                var cartId = GenerateCartId();

                // 获取当前时间
                var currentTime = DateTime.UtcNow;
                currentTime = ConvertToBeijingTime(currentTime);
                // 创建新的 Cart 实例
                var newCart = new Cart
                {
                    CartId = cartId,
                    AccountId = accountId,
                    CreatedTime = currentTime,
                    UpdatedTime = currentTime
                };

                // 保存 Cart 到仓库中
                await _cartRepository.AddAsync(newCart);

                // 返回 CartResponseDto
                return new CartResponseDto
                {
                    success = true,
                    msg = "Cart created successfully",
                    response = new CartResponseDto.CartResponse
                    {
                        cartId = cartId,
                        createTime = currentTime,
                        updateTime = currentTime
                    }
                };
            }
            catch (Exception ex)
            {
                return new CartResponseDto
                {
                    success = false,
                    msg = ex.ToString()
                };
            }


        }
        public async Task<CartResponseDto> DeleteCartAsync(string accountId)
        {
            try
            {
                var existedCart = (await _cartRepository.FindByConditionAsync(cart => cart.AccountId == accountId)).FirstOrDefault();
                if (existedCart != null)
                {
                    await _cartRepository.DeleteAsync(existedCart);

                    // 返回 CartResponseDto
                    return new CartResponseDto
                    {
                        success = true,
                        msg = "Cart Deleted Successfully!",
                        response = new CartResponseDto.CartResponse
                        {
                            cartId = existedCart.CartId,
                            createTime = existedCart.CreatedTime,
                            updateTime = existedCart.UpdatedTime
                        }
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return new CartResponseDto
                {
                    success = false,
                    msg = ex.ToString()
                };
            }
        }
    }
}
