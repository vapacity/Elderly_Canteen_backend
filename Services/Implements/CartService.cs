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
        private readonly IGenericRepository<OrderInf> _orderInfoRepository;

        public CartService(IGenericRepository<Cart> cartRepository, IGenericRepository<CartItem> cartItemRepository, IGenericRepository<OrderInf> orderInfoRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _orderInfoRepository = orderInfoRepository;
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
                var existedCart = await _cartRepository.FindByConditionAsync(cart => cart.AccountId == accountId);
                //查看所有用户已存在的购物车
                foreach(var excart in existedCart)
                {
                    //获得购物车中尚未出现在orderInfo中的，避免获得已经收纳为订单的购物车
                    var result = await _orderInfoRepository.FindByConditionAsync(o => o.CartId == excart.CartId);
                    if(result.Any())
                    {
                        continue;
                    }
                    else
                    {
                        //返回已经存在但是不在orderinfo中的购物车
                        return new CartResponseDto
                        {
                            success =true,
                            msg = "Cart already existed!",
                            response = new CartResponseDto.CartResponse
                            {
                                cartId = excart.CartId,
                                createTime = excart.CreatedTime,
                                updateTime = excart.UpdatedTime,
                            }
                            
                        };
                    }
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
                // 查找与 accountId 关联的所有购物车
                var existedCarts = await _cartRepository.FindByConditionAsync(cart => cart.AccountId == accountId);

                if (existedCarts.Any())
                {
                    // 遍历所有关联的购物车
                    foreach (var cart in existedCarts)
                    {
                        // 检查购物车是否存在于 orderInfo 中
                        var orderInfo = await _orderInfoRepository.FindByConditionAsync(o => o.CartId == cart.CartId);

                        if (!orderInfo.Any())
                        {
                            // 如果购物车不在 orderInfo 中，则删除该购物车
                            await _cartRepository.DeleteAsync(cart);

                            // 返回成功消息
                            return new CartResponseDto
                            {
                                success = true,
                                msg = "成功删除购物车",
                                response = new CartResponseDto.CartResponse
                                {
                                    cartId = cart.CartId,
                                    createTime = cart.CreatedTime,
                                    updateTime = cart.UpdatedTime
                                }
                            };
                        }
                    }

                    // 如果所有的购物车都关联了订单，则返回错误消息
                    return new CartResponseDto
                    {
                        success = false,
                        msg = "所有购物车都已经关联到了订单中"
                    };
                }
                else
                {
                    // 如果找不到任何购物车，返回错误消息
                    return new CartResponseDto
                    {
                        success = false,
                        msg = "没有找到与用户关联的购物车"
                    };
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回错误消息
                return new CartResponseDto
                {
                    success = false,
                    msg = ex.ToString()
                };
            }
        }


    }
}
