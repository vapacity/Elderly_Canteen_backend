using Elderly_Canteen.Data.Dtos.Cart;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Elderly_Canteen.Services.Implements
{
    public class CartService : ICartService
    {
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly IGenericRepository<CartItem> _cartItemRepository;
        private readonly IGenericRepository<OrderInf> _orderInfoRepository;
        private readonly IGenericRepository<Weekmenu> _weekMenuRepository;
        private readonly IGenericRepository<Dish> _dishRepository;
        private readonly IOrderService _orderService;
        private readonly IFinanceService _financeService;
        private readonly IRepoService _repoService;
        private readonly INotificationService _notificationService;
        private readonly ModelContext _dbContext;
        public CartService(IGenericRepository<Cart> cartRepository,
            IGenericRepository<CartItem> cartItemRepository,
            IGenericRepository<OrderInf> orderInfoRepository,
            IGenericRepository<Weekmenu> weekMenuRepository,
            IOrderService orderService,
            IFinanceService financeService,
            IRepoService repoService,
            INotificationService notificationService,
            ModelContext dbContext,
            IGenericRepository<Dish> dishRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _orderInfoRepository = orderInfoRepository;
            _weekMenuRepository = weekMenuRepository;
            _orderService = orderService;
            _financeService = financeService;
            _repoService = repoService;
            _notificationService = notificationService;
            _dbContext = dbContext;
            _dishRepository = dishRepository;
        }
        private async Task<Cart> IsCartValid(string cartId, string accountId)
        {
            // 1. 验证 CartId 是否存在且不在 OrderInfo 表中
            var cart = (await _cartRepository.FindByConditionAsync(c => c.CartId == cartId)).FirstOrDefault();

            if (cart == null)
            {
                // CartId 不存在
                return null;
            }

            if (cart.AccountId != accountId)
            {
                // CartId 和传入的 accountId 不匹配
                return null;
            }

            var orderInfoExists = (await _orderInfoRepository.FindByConditionAsync(o => o.CartId == cartId)).Any();
            if (orderInfoExists)
            {
                // CartId 已经在 OrderInfo 中
                return null;
            }
            return cart;
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
                // 获取今天的日期
                var today = DateTime.Today;

                // 查找创建时间为今天的购物车
                var existedCart = await _cartRepository.FindByConditionAsync(cart => cart.AccountId == accountId && cart.CreatedTime.Date == today);

                // 查看所有用户今天已存在的购物车
                foreach (var excart in existedCart)
                {
                    // 获得购物车中尚未出现在 orderInfo 中的，避免获得已经收纳为订单的购物车
                    var result = await _orderInfoRepository.FindByConditionAsync(o => o.CartId == excart.CartId);
                    if (result.Any())
                    {
                        continue;
                    }
                    else
                    {
                        // 返回今天创建的但是不在 orderInfo 中的购物车
                        return new CartResponseDto
                        {
                            success = true,
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
                var cart = await IsCartValid(dto.CartId, accountId);
                // 1. 验证 CartId 是否存在且不在 OrderInfo 表中
                if (cart == null)
                {
                    return new CartItemResponseDto
                    {
                        Success = false,
                        Message = "cartId 或 accountId 有误"
                    };
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

        public async Task<CartItemResponseDto> EnsureCartItem(string cartId, bool deliver_or_dining, string accountId)
        {
            // 查找与用户关联的购物车
            // 查找购物车item
            // 插入orderinfo表中

            // **库存相关**
            // 1. 更新weekmenu: 减去weekmenu中当天与dishId相同的那一项的stock，减去的数量等于这个当前订单的quantity数量，检查stock剩余量是否小于10，如果小于则提醒
            // 2. 更新weekmenu: 临时想到，weekmenu中的stock应该每天早上更新，自动设置当天所有菜品的stock为50，需要计算repository中的食材能否做出50份，如果不行则需要返回提示（如何提示前端呢），如果够则从repository中直接删除对应的量，
            //                  删除逻辑：查找formula表获得对应的ingredientId和quantity，在repository查到ingredientId，从过期时间最早的开始减去，如果该过期时间已经全部消耗完，那么删除这一项，开始删其他过期时间的
            //                  同理，weekmenu中的stock可以被修改，当weekmenu被修改的时候就应该执行检查逻辑，和上面的一致。总之我认为需要写一个过程和一个trigger on update
            // 3. 更新repository: 这部分的执行应该在stock变化后，每当stock成功增加，也就代表消耗这些食材被消耗用于做这些菜。也就是删除逻辑。
            // 
            // 

            // **财务相关**
            // 1. 对于这个任务，需要检验身份，如果是普通用户，那么扣除其用户余额（需要写余额不足的逻辑，这个应该是放在最开始的），添加到finance表中这项记录，
            // 2. 如果是老人，那么需要将price*0.8的价格作为该用户从用户余额中扣除，另外0.2从老人的补贴中扣除，finance表中的价格应该计为用户余额中扣除的前，而不包含补贴。
            // 

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 验证 CartId 是否存在且不在 OrderInfo 表中
                    var cart = await IsCartValid(cartId, accountId);
                    if (cart == null)
                    {
                        return new CartItemResponseDto
                        {
                            Success = false,
                            Message = "cartId 或 accountId 有误"
                        };
                    }

                    // 2. 检查获得购物车，并检查购物车是否为空
                    var cartItems = (await _cartItemRepository.FindByConditionAsync(ci => ci.CartId == cartId)).ToList();
                    if (!cartItems.Any())
                    {
                        return new CartItemResponseDto
                        {
                            Success = false,
                            Message = "购物车为空"
                        };
                    }

                    // 3. 调用库存管理服务检查并减少库存
                    foreach (var cartItem in cartItems)
                    {
                        bool repoReduced = await _repoService.CheckAndReduceStockAsync(cartItem.DishId, cartItem.Week, cartItem.Quantity);
                        if (!repoReduced)
                        {
                            await transaction.RollbackAsync();
                            return new CartItemResponseDto { Success = false, Message = $"库存不足：{cartItem.DishId}" };
                        }
                    }

                    // 5. 调用财务管理服务扣除用户余额
                    decimal totalPrice = await _orderService.CalculateTotalPrice(cartItems);
                    dynamic response = await _financeService.DeductBalanceAsync(accountId, totalPrice);

                    if (response.Success == false)
                    {
                        await transaction.RollbackAsync();
                        return new CartItemResponseDto { Success = false, Message = response.Msg };
                    }

                    // 4. 调用订单管理服务生成订单
                    var orderInfo = await _orderService.CreateOrderAsync(cartId, accountId, deliver_or_dining, response.FinanceId, cartItems);
                    if (orderInfo.Success == false)
                    {
                        await transaction.RollbackAsync();
                        return new CartItemResponseDto
                        {
                            Success = false,
                            Message = orderInfo.Msg
                        };
                    }

                    // 6. 提交事务
                    await transaction.CommitAsync();

                    return new CartItemResponseDto { Success = true, Message = "订单处理成功" };
                }
                catch (Exception ex)
                {
                    // 回滚事务
                    await transaction.RollbackAsync();
                    // 记录异常日志
                    throw;  // 重新抛出异常，以便上层处理
                }
            }

        }
        
        public async Task<CartItemsDto> GetCartItemsAsync(string cartId, string accountId)
        {
            // 使用 IsCartValid 方法验证 CartId 和 AccountId
            var cart = await IsCartValid(cartId, accountId);

            if (cart == null)
            {
                return new CartItemsDto
                {
                    Success = false,
                    Message = "未找到匹配的购物车或购物车已经关联订单",
                    Menu = new List<Menu>()
                };
            }

            // 查找与购物车相关的项目
            var cartItems = await _cartItemRepository.FindByConditionAsync(ci => ci.CartId == cartId);
            if (!cartItems.Any())
            {
                return new CartItemsDto
                {
                    Success = false,
                    Message = "购物车为空",
                    Menu = new List<Menu>()
                };
            }

            // 创建返回的 Menu 项列表
            var menuItems = new List<Menu>();
            foreach (var cartItem in cartItems)
            {
                var day = MapDayOfWeekToShortString(DateTime.Now.DayOfWeek);
                var weekMenu = await _weekMenuRepository.FindByConditionAsync(wm => wm.DishId == cartItem.DishId && wm.Week == cartItem.Week && wm.Day == day);
                var discountPrice = weekMenu.FirstOrDefault()?.DisPrice??0m;
                var dish = await _dishRepository.GetByIdAsync(cartItem.DishId);
                if (dish != null)
                {
                    menuItems.Add(new Menu
                    {
                        DishId = dish.DishId,
                        DishName = dish.DishName,
                        DishPrice = dish.Price,
                        DiscountPrice = discountPrice,
                        ImageUrl = dish.ImageUrl,
                        Quantity = cartItem.Quantity
                    });
                }
            }

            // 返回购物车项目信息
            return new CartItemsDto
            {
                Success = true,
                Message = "成功获取购物车项目",
                Menu = menuItems
            };
        }

        public async Task<bool> ClearItemsAsync(string cartId)
        {
            // 查找指定 cartId 的购物车
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null)
            {
                return false; // 如果购物车不存在，则返回 false
            }

            // 这里你可以添加额外的验证逻辑以确保购物车是有效的
            // 例如，如果 "有效" 表示购物车没有被关联到任何订单中，你可以进行如下检查：
            var orderCount = await _orderInfoRepository.CountAsync(o => o.CartId == cartId);
            if (orderCount > 0)
            {
                return false; // 如果购物车已关联到订单，则不删除并返回 false
            }

            // 删除该购物车下的所有购物车项
            await _cartItemRepository.DeleteByConditionAsync(ci => ci.CartId == cartId);


            return true; // 如果成功删除，则返回 true
        }


        public async Task DeleteUnassociatedCartsAsync()
        {
            var today = DateTime.Today;

            // 首先查找所有创建日期不为今天的购物车
            var cartsToCheck = await _cartRepository.FindByConditionAsync(cart => cart.CreatedTime.Date != today);

            var cartsToDelete = new List<Cart>();

            foreach (var cart in cartsToCheck)
            {
                // 检查每个购物车是否未关联订单
                var orderCount = await _orderInfoRepository.CountAsync(o => o.CartId == cart.CartId);
                if (orderCount == 0)
                {
                    cartsToDelete.Add(cart);
                }
            }

            // 删除符合条件的购物车及其项
            foreach (var cart in cartsToDelete)
            {
                // 删除该购物车下的所有购物车项
                await _cartItemRepository.DeleteByConditionAsync(ci => ci.CartId == cart.CartId);

                // 删除购物车本身
                await _cartRepository.DeleteAsync(cart.CartId);
            }
        }


    }
}

