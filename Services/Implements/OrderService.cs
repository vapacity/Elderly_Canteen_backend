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


        public OrderService(
            IGenericRepository<Weekmenu> weekMenuRepository,
            IGenericRepository<Dish> dishRepository,
            IGenericRepository<OrderInf> orderInfRepository,
            IGenericRepository<Account> accountRepository)
        {
            _weekMenuRepository = weekMenuRepository;
            _dishRepository = dishRepository;
            _orderInfRepository = orderInfRepository;
            _accountRepository = accountRepository;
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
            int daysToMonday = (int)dayOfWeek - (int)DayOfWeek.Monday;
            DateTime weekStartDate = today.AddDays(-daysToMonday).Date;
            // 将 DayOfWeek 映射为 Mon, Tue, Wed 等形式
            string dayString = MapDayOfWeekToShortString(dayOfWeek);

            // 查找符合条件的 WeekMenu 项目
            var weekMenus = (await _weekMenuRepository.FindByConditionAsync(wm => wm.Week == weekStartDate && wm.Day == dayString)).ToList();

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

        public async Task<OrderInfoDto> CreateOrderAsync(string cartId, string accountId, bool deliver_or_dining, string financeId, List<CartItem> cartItems)
        {
            // 1. 初始化变量，用于计算总价
            decimal totalPrice = 0;
            List<OrderDish> orderDishes = new List<OrderDish>();
            totalPrice = await CalculateTotalPrice(cartItems);
           /* // 获取今日菜单
            var menuToday = await GetMenuToday();
            var todayMenuItems = menuToday.Menu.ToDictionary(m => m.DishId, m => m);

            // 2. 遍历购物车项，计算总价并创建订单菜品项
            foreach (var cartItem in cartItems)
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
            if(account.Address == null)
            {
                return new OrderInfoDto
                {
                    Success = false,
                    Msg = "account address is null"
                };
            }
            decimal bonus = 0;
            if (account.Identity == "senior")
                bonus = totalPrice * 0.2m;
            // 3. 生成订单记录
            var order = new OrderInf
            {
                OrderId = Guid.NewGuid().ToString(), // 生成唯一订单ID
                DeliverOrDining = deliver_or_dining ? "D" : "I", // 根据传入参数设置
                CartId = cartId,
                Status = "待处理", // 订单初始状态
                Bonus = bonus, // 初始为0
                Remark = "无评论", // 根据业务逻辑填充
                FinanceId =financeId, // 假设生成一个财务ID
            };

            // 4. 保存订单记录到OrderInfo表中
            await _orderInfRepository.AddAsync(order);

            // 5. 创建并返回 OrderInfoDto
            var orderInfoDto = new OrderInfoDto
            {
                Msg = "订单创建成功",
                Success = true,
                Response = new OrderItem
                {
                    CusAddress = account.Address, // 根据业务逻辑填充
                    DeliverOrDining = deliver_or_dining,
                    DeliverStatus = "待配送", // 初始配送状态
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

        // 3. 返回订单信息

    }

}

