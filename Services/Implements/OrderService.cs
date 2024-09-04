namespace Elderly_Canteen.Services.Implements
{
    using Elderly_Canteen.Data.Entities;
    using Elderly_Canteen.Data.Repos;
    using Elderly_Canteen.Services.Interfaces;
    using Elderly_Canteen.Data.Dtos.Order;
    public class OrderService:IOrderService
    {
        private readonly IGenericRepository<Weekmenu> _weekMenuRepository;
        private readonly IGenericRepository<Dish> _dishRepository;
        private readonly IGenericRepository<OrderInf> _orderInfRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<Finance> _financeRepository;
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly IGenericRepository<CartItem> _cartItemRepository;
        private readonly IGenericRepository<DeliverOrder> _deliverOrderRepository;

        public OrderService(
            IGenericRepository<Weekmenu> weekMenuRepository,
            IGenericRepository<Dish> dishRepository,
            IGenericRepository<Finance> financeRepository,
            IGenericRepository<OrderInf> orderInfRepository,
            IGenericRepository<Account> accountRepository,
            IGenericRepository<Cart> cartRepository,
            IGenericRepository<CartItem> cartItemRepository,
            IGenericRepository<DeliverOrder> deliverOrderRepository)
        {
            _weekMenuRepository = weekMenuRepository;
            _dishRepository = dishRepository;
            _financeRepository = financeRepository;
            _orderInfRepository = orderInfRepository;
            _accountRepository = accountRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _deliverOrderRepository = deliverOrderRepository;
        }
        //计算当前周数
        private DateTime GetWeekStartDate()
        {
            DateTime today = DateTime.Now;
            DayOfWeek dayOfWeek = today.DayOfWeek;
            int daysToMonday = (int)dayOfWeek - (int)DayOfWeek.Monday;
            DateTime weekStartDate = today.AddDays(-daysToMonday).Date; // 获取本周的周一
            return weekStartDate;
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

        public async Task<MenuResponseDto> GetMenuToday()
        {
            // 获取当前日期和时间
            DateTime today = DateTime.Now;

            // 计算本周的周一日期
            DayOfWeek dayOfWeek = today.DayOfWeek;
            
            // 获得当天日期
            var todayDate = today.Date;
            // 将 DayOfWeek 映射为 Mon, Tue, Wed 等形式
            string dayString = MapDayOfWeekToShortString(dayOfWeek);

            // 查找符合条件的 WeekMenu 项目
            var weekMenus = (await _weekMenuRepository.FindByConditionAsync(wm => wm.Week == todayDate && wm.Day == dayString)).ToList();

            // 准备存放菜单信息的列表
            var menuList = new List<Menu>();

            // 遍历每个 WeekMenu 项，查询关联的 Dish 信息
            foreach (var weekMenu in weekMenus)
            {
                // 使用 DishId 从 DishRepository 中获取菜品信息
                var dish = await _dishRepository.GetByIdAsync(weekMenu.DishId);

                if (dish != null)
                {
                    // 将菜品信息映射到 Menu DTO 中
                    menuList.Add(new Menu
                    {
                        DishId = dish.DishId,
                        //缺种类名
                        Category = dish.CateId,
                        DishName = dish.DishName,
                        DishPrice = dish.Price,
                        DiscountPrice = weekMenu.DisPrice,
                        ImageUrl = dish.ImageUrl,
                        Sales = weekMenu.Sales,
                        Stock = weekMenu.Stock
                    });
                }
            }

            // 返回封装好的 MenuResponseDto
            var response = new MenuResponseDto
            {
                Menu = menuList,
                Message = menuList.Any() ? "成功获取菜单" : "今日无菜单",
                Success = menuList.Any()
            };

            return response;
        }

        public async Task<decimal> CalculateTotalPrice(List<CartItem> cartItems)
        {
            decimal totalPrice = 0;

            // 获取今日菜单
            var menuToday = await GetMenuToday();
            var todayMenuItems = menuToday.Menu.ToDictionary(m => m.DishId, m => m);

            // 遍历购物车项，计算总价
            foreach (var cartItem in cartItems)
            {
                // 查找菜品在今日菜单中的信息
                if (todayMenuItems.TryGetValue(cartItem.DishId, out var menuItem))
                {
                    // 使用折扣价（如果有），否则使用正常价格
                    decimal itemPrice = menuItem.DiscountPrice == 0 ? menuItem.DishPrice : menuItem.DiscountPrice;
                    decimal itemTotal = cartItem.Quantity * itemPrice;

                    totalPrice += itemTotal;
                }
                else
                {
                    // 如果某个菜品不在今日菜单中，可以选择抛出异常或者继续处理
                    throw new Exception($"菜品ID {cartItem.DishId} 不在今日菜单中，无法计算价格。");
                }
            }

            return totalPrice;
        }

        public async Task<OrderInfoDto> CreateOrderAsync(string cartId, string accountId, string? newAddress,bool deliver_or_dining, string financeId, List<CartItem> cartItems)
        {
            // 1. 初始化变量，用于计算总价
            decimal totalPrice = 0;
            List<OrderDish> orderDishes = new List<OrderDish>();
            totalPrice = await CalculateTotalPrice(cartItems);
           /* // 获取今日菜单
            var menuToday = await GetMenuToday();
            var todayMenuItems = menuToday.Menu.ToDictionary(m => m.DishId, m => m);

            // 2. 遍历购物车项，计算总价并创建订单菜品项
            foreach (var cartItem in cartItems)S
            {
                // 查找菜品在今日菜单中的信息
                if (todayMenuItems.TryGetValue(cartItem.DishId, out var menuItem))
                {
                    decimal itemTotal = cartItem.Quantity * (menuItem.DiscountPrice==0 ? menuItem.DishPrice : menuItem.DiscountPrice);
                    totalPrice += itemTotal;

                    // 创建订单菜品项
                    var orderDish = new OrderDish
                    {
                        DishName = menuItem.DishName,
                        Picture = menuItem.ImageUrl,
                        Price = menuItem.DiscountPrice == 0 ? menuItem.DishPrice : menuItem.DiscountPrice,
                        Quantity = cartItem.Quantity
                    };

                    orderDishes.Add(orderDish);
                }
                else
                {
                    throw new Exception($"菜品ID {cartItem.DishId} 不在今日菜单中，无法生成订单。");
                }
            }*/

            var account = await _accountRepository.GetByIdAsync(accountId);
            if(account.Address == null && newAddress == null)
            {
                return new OrderInfoDto
                {
                    Success = false,
                    Msg = "account address is null"
                };
            }
            var address = newAddress ?? account.Address;

            decimal bonus = 0;
            if (account.Identity == "senior")
                bonus = totalPrice * 0.2m;
            // 3. 生成订单记录
            var order = new OrderInf
            {
                OrderId = await GenerateOrderIdAsync(), // 生成唯一订单ID
                DeliverOrDining = deliver_or_dining ? "D" : "I", // 根据传入参数设置
                CartId = cartId,
                Status = "待处理", // 订单初始状态
                Bonus = bonus, // 初始为0
                Remark = "无评论", // 根据业务逻辑填充
                FinanceId =financeId, 
            };
            

            
            // 4. 保存订单记录到OrderInfo表中
            await _orderInfRepository.AddAsync(order);
            // 3.5 生成配送订单
            if (deliver_or_dining)
            {
                var newOrder = new DeliverOrder
                {
                    OrderId = order.OrderId,
                    CartId = cartId,
                    DeliverPhone = "未接单",
                    CustomerPhone = account.Phonenum,
                    CusAddress = address,
                    DeliverStatus = "未接单"
                };
                await _deliverOrderRepository.AddAsync(newOrder);

            }
            // 5. 创建并返回 OrderInfoDto
            var orderInfoDto = new OrderInfoDto
            {
                Msg = "订单创建成功",
                Success = true,
                Response = new OrderItem
                {
                    CusAddress = address ?? "error", // 根据业务逻辑填充
                    DeliverOrDining = deliver_or_dining,
                    DeliverStatus = deliver_or_dining?"未接单":"堂食", // 初始配送状态
                    Money = totalPrice,
                    OrderDishes = orderDishes,
                    Remark = order.Remark,
                    Status = order.Status,
                    Subsidy = bonus, // 初始补贴为0
                    UpdatedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            return orderInfoDto;
        }

        private async Task<string> GenerateOrderIdAsync()
        {
            // 获取当前日期部分：20240708
            string datePart = DateTime.Now.ToString("yyyyMMdd");

            // 获取当前日期的起始和结束时间
            DateTime todayStart = DateTime.Today;
            DateTime todayEnd = todayStart.AddDays(1).AddTicks(-1);

            // 从 Finance 表中获取当天的订单数量（FinanceDate 为当天的记录）
            int financeCount = await _financeRepository.CountAsync(f => f.FinanceDate >= todayStart && f.FinanceDate <= todayEnd && f.FinanceType == "点单");

            // 生成四位的订单计数部分，从0001开始
            string orderCountPart = (financeCount + 1).ToString("D4");

            // 组合日期部分和订单计数部分，生成最终的订单ID
            string orderId = datePart + orderCountPart;

            return orderId;
        }
        // 3. 返回订单信息

        public async Task<GetOrderResponseDto> GetHistoryOrderInfoAsync(string accountId)
        {
            // 1. 查找所有与此用户相关的 finance 记录
            var financeList = await _financeRepository.FindByConditionAsync(f => f.AccountId == accountId && f.FinanceType == "点单");

            // 2. 如果没有记录，返回空的结果
            if (!financeList.Any())
            {
                return new GetOrderResponseDto { Success = false, Msg = "没有历史订单", Response = Array.Empty<OrderItem>() };
            }

            // 创建一个 List 来存储所有的 OrderItem
            var orderItemList = new List<OrderItem>();

            // 3. 遍历所有找到的 finance 记录，获取订单、购物车和购物车项目
            foreach (var finance in financeList)
            {
                // 通过 financeId 查找 orderInfo
                var orderInfo = await _orderInfRepository.FindByConditionAsync(o => o.FinanceId == finance.FinanceId);
                if (orderInfo == null || !orderInfo.Any())
                {
                    continue;
                }

                var order = orderInfo.FirstOrDefault();

                // 通过 cartId 获取购物车信息
                var cart = await _cartRepository.GetByIdAsync(order.CartId);

                // 获取购物车中的所有项目
                var cartItems = await _cartItemRepository.FindByConditionAsync(ci => ci.CartId == cart.CartId);

                // 组装 orderDishes 信息
                var orderDishes = new List<OrderDish>();
                foreach (var cartItem in cartItems)
                {
                    var dish = await _dishRepository.GetByIdAsync(cartItem.DishId);  // 假设有 DishRepository 来获取菜品信息

                    if (dish != null)
                    {
                        orderDishes.Add(new OrderDish{
                            DishName = dish.DishName,
                            Picture = dish.ImageUrl,
                            Price = cartItem.Quantity * dish.Price, // 假设价格保存在 Dish 中
                            Quantity = cartItem.Quantity
                        });
                    }
                }
                
                var deliverOrder = await _deliverOrderRepository.GetByIdAsync(order.OrderId);
                // 4. 构建 OrderItem
                var orderItem = new OrderItem
                {
                    CusAddress =  deliverOrder != null? deliverOrder.CusAddress: "堂食",  // 获取客户地址
                    DeliverOrDining = order.DeliverOrDining == "D",  // 处理外送或堂食
                    DeliverStatus = deliverOrder !=null? deliverOrder.DeliverStatus:"堂食",  // 配送状态可以根据业务逻辑设置
                    Money = finance.Price,
                    OrderDishes = orderDishes,
                    Remark = order.Remark ?? "no remark",
                    Status = order.Status,
                    Subsidy = order.Bonus,  // 补贴
                    UpdatedTime = finance.FinanceDate.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // 将 orderItem 添加到列表中
                orderItemList.Add(orderItem);
            }

            // 5. 构建 GetOrderResponseDto，并将 OrderItem 列表转换为数组返回
            return new GetOrderResponseDto
            {
                Success = true,
                Msg = "成功获取历史订单",
                Response = orderItemList.ToArray()  // 转换为数组
            };
        }

    }

}

