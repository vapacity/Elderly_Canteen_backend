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

        public OrderService(
            IGenericRepository<Weekmenu> weekMenuRepository,
            IGenericRepository<Dish> dishRepository)
        {
            _weekMenuRepository = weekMenuRepository;
            _dishRepository = dishRepository;
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
    }
}
