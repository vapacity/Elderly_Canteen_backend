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
        private readonly IGenericRepository<Weekmenu> _weekMenuRepository;

        public CartService(IGenericRepository<Cart> cartRepository, IGenericRepository<CartItem> cartItemRepository, IGenericRepository<OrderInf> orderInfoRepository, IGenericRepository<Weekmenu> weekMenuRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _orderInfoRepository = orderInfoRepository;
            _weekMenuRepository = weekMenuRepository;   
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
        // 映射 DayOfWeek 到 Mon, Tue, Wed 等字符串
        private string MapDayOfWeekToShortString(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Mon",
                DayOfWeek.Tuesday => "Tue",
                DayOfWeek.Wednesday => "Wed",
                DayOfWeek.Thursday => "Thu",
                DayOfWeek.Friday => "Fri",
                DayOfWeek.Saturday => "Sat",
                DayOfWeek.Sunday => "Sun",
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
            };
        }
        public async Task<CartItemResponseDto> UpdateCartItemAsync(CartItemRequestDto dto, string accountId)
        {
            try
            {
                // 1. 验证 CartId 是否存在且不在 OrderInfo 表中
                var cart = (await _cartRepository.FindByConditionAsync(c => c.CartId == dto.CartId)).FirstOrDefault();

                if (cart == null)
                {
                    // CartId 不存在
                    return new CartItemResponseDto { Success = false, Message = "CartId无效" };
                }

                if (cart.AccountId != accountId)
                {
                    // CartId 和传入的 accountId 不匹配
                    return new CartItemResponseDto { Success = false, Message = "CartId与AccountId不匹配" };
                }

                var orderInfoExists = (await _orderInfoRepository.FindByConditionAsync(o => o.CartId == dto.CartId)).Any();
                if (orderInfoExists)
                {
                    // CartId 已经在 OrderInfo 中
                    return new CartItemResponseDto  { Success = false, Message = "CartId无效" };
                }

                // 2. 验证 DishId 是否在今日菜单中
                DateTime today = DateTime.Now;
                DayOfWeek dayOfWeek = today.DayOfWeek;
                int daysToMonday = (int)dayOfWeek - (int)DayOfWeek.Monday;
                DateTime weekStartDate = today.AddDays(-daysToMonday).Date;
                string dayString = MapDayOfWeekToShortString(dayOfWeek);

                var weekMenu = (await _weekMenuRepository
                    .FindByConditionAsync(wm => wm.Week == weekStartDate && wm.Day == dayString && wm.DishId == dto.DishId))
                    .FirstOrDefault();

                if (weekMenu == null)
                {
                    // DishId 不存在于今日菜单
                    return new CartItemResponseDto { Success = false, Message = $"今天没有 {dto.DishId} 的菜单项" };
                }

                // 3. 检查库存是否足够
                if (dto.Quantity > weekMenu.Stock)
                {
                    // 请求的数量超过库存
                    return new CartItemResponseDto { Success = false, Message = "超过库存，操作失败" };
                }

                // 4. 更新或添加购物车项
                var cartItem = (await _cartItemRepository.FindByConditionAsync(ci => ci.CartId == dto.CartId && ci.DishId == dto.DishId)).FirstOrDefault();

                if (cartItem == null)
                {
                    // 如果购物车中不存在该菜品项，则添加新项
                    cartItem = new CartItem
                    {
                        CartId = dto.CartId,
                        DishId = dto.DishId,
                        Week = weekStartDate,
                        Quantity = (int)dto.Quantity
                    };
                    await _cartItemRepository.AddAsync(cartItem);
                }
                else
                {
                    // 如果存在，则更新数量
                    cartItem.Quantity = (int)dto.Quantity;
                    await _cartItemRepository.UpdateAsync(cartItem);
                }

                // 更新购物车的更新时间
                cart.UpdatedTime = DateTime.Now;
                await _cartRepository.UpdateAsync(cart);

                return new CartItemResponseDto { Success = true, Message = "购物车项更新成功" };
            }
            catch (Exception ex)
            {
                // 这里可以记录日志或者其他处理
                Console.WriteLine(ex.Message);
                return new CartItemResponseDto { Success = false, Message = $"更新购物车项时发生错误: {ex.Message}" };
            }
        }
        private DateTime GetWeekStartDate()
        {
            DateTime today = DateTime.Now;
            DayOfWeek dayOfWeek = today.DayOfWeek;
            int daysToMonday = (int)dayOfWeek - (int)DayOfWeek.Monday;
            DateTime weekStartDate = today.AddDays(-daysToMonday).Date; // 获取本周的周一
            return weekStartDate;
        }
        public async Task<CartItemResponseDto> DeleteCartItem(DeleteRequestDto dto, string accountId)
        {
            try
            {
                // 1. 验证 CartId 是否存在且属于当前用户
                var cart = (await _cartRepository.FindByConditionAsync(c => c.CartId == dto.CartId && c.AccountId == accountId)).FirstOrDefault();

                if (cart == null)
                {
                    return new CartItemResponseDto
                    {
                        Success = false,
                        Message = "CartId无效或与AccountId不匹配"
                    };
                }

                // 2. 验证 DishId 是否存在于购物车项中
                var weekStartDate = GetWeekStartDate(); // 获取当前周的周一日期，作为 Week 的值
                var cartItem = (await _cartItemRepository.FindByConditionAsync(ci => ci.CartId == dto.CartId && ci.DishId == dto.DishId && ci.Week == weekStartDate)).FirstOrDefault();

                if (cartItem == null)
                {
                    return new CartItemResponseDto
                    {
                        Success = false,
                        Message = $"购物车中未找到菜品ID: {dto.DishId}"
                    };
                }

                // 3. 使用复合主键删除购物车项
                await _cartItemRepository.DeleteByCompositeKeyAsync<CartItem>(dto.CartId, dto.DishId, weekStartDate);

                // 4. 更新购物车的更新时间
                cart.UpdatedTime = DateTime.Now;
                await _cartRepository.UpdateAsync(cart);

                return new CartItemResponseDto
                {
                    Success = true,
                    Message = "购物车项删除成功"
                };
            }
            catch (Exception ex)
            {
                // 记录异常日志（如需要），并返回错误信息
                return new CartItemResponseDto
                {
                    Success = false,
                    Message = $"删除购物车项时发生错误: {ex.Message}"
                };
            }
        }

/*        public async Task<CartItemResponseDto> EnsureCartItem(string cartId,string accountId)
        {

        }*/
    }
}
