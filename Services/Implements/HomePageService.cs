using Elderly_Canteen.Data.Dtos.Dish;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using System.Text;
using Elderly_Canteen.Data.Dtos.Order;
using static System.Runtime.InteropServices.JavaScript.JSType;


public class HomePageService : IHomePageService
{
    private readonly IGenericRepository<Dish> _dishRepository;
    private readonly IGenericRepository<Weekmenu> _weekmenuRepository;
    private readonly IGenericRepository<OrderReview> _orderReviewRepository;

    public HomePageService(IGenericRepository<Dish> dishRepository, IGenericRepository<Weekmenu> weekmenuRepository, IGenericRepository<OrderReview> orderReviewRepository)
    {
        _dishRepository = dishRepository;
        _weekmenuRepository = weekmenuRepository;
        _orderReviewRepository = orderReviewRepository;
    }

    public async Task<List<object>> GetDishesAsync()
    {
        DateTime today = DateTime.Today;
        int daysToMonday = (int)today.DayOfWeek- (int)DayOfWeek.Monday;
        DateTime monday = today.AddDays(-daysToMonday);
        DateTime sunday = monday.AddDays(6);



        // 预计算日期值
        var mondayDate = monday.Date;
        var sundayDate = sunday.Date;

        // 获取销量数据
        var dishSales = await _weekmenuRepository.GetAll()
            .Where(wm => wm.Week >= mondayDate && wm.Week <= sundayDate)
            .GroupBy(wm => wm.DishId)
            .Select(group => new {
                DishId = group.Key,
                WeekSales = group.Sum(wm => wm.Sales)
            }).ToListAsync();

        // 转换为字典以提高访问效率
         var salesDictionary = dishSales.ToDictionary(ds => ds.DishId, ds => ds.WeekSales);

        // 根据有效的 DishId 查询相应的菜品
         var result = await _dishRepository.GetAll()
                             .Select(d => new {
                                 dishName = d.DishName,
                                 picture = d.ImageUrl,
                                 category = d.Cate.CateName,
                                 sale = salesDictionary.ContainsKey(d.DishId) ? salesDictionary[d.DishId] : 0,
                                 price = d.Price,
                             })
                             .ToListAsync();
        return result.Cast<object>().ToList();
    }

    public async Task<List<object>> GetThisWeekMenu()
    {
/*        // 加载默认静态图片的 byte[] 数据
        byte[] defaultPicture = await LoadDefaultPictureAsync();*/

        // 从数据库中查询数据
        var result = await (from dish in _dishRepository.GetAll()
                            join weekmenu in _weekmenuRepository.GetAll()
                            on dish.DishId equals weekmenu.DishId
                            select new 
                            {
                                dishName = dish.DishName,
                                picture = dish.ImageUrl,
                                week = weekmenu.Week.ToString("yyyy-MM-dd"),
                                price = weekmenu.DisPrice > 0 ? weekmenu.DisPrice : dish.Price,
                                disPrice = weekmenu.DisPrice
                            }).ToListAsync();

        var finalResult = result.ToList<object>();

        return finalResult;
    }
    private async Task<byte[]> LoadDefaultPictureAsync()
    {
        // 获取项目根目录
        string rootPath = Directory.GetCurrentDirectory();

        // 拼接相对路径以指向 uploads 目录
        string defaultPicturePath = Path.Combine(rootPath, "uploads", "default_dish.png");

        // 异步读取文件为 byte[]，并返回
        return await File.ReadAllBytesAsync(defaultPicturePath);
    }

    public async Task<dynamic> GetThisDayDiscountMenu()
    {
        // 获取当前日期和时间
        DateTime today = DateTime.Today;

        DateTime startDate = today;
        DateTime endDate = startDate.AddDays(1).AddTicks(-1);

        // 查找符合条件的 WeekMenu 项目
        var weekMenus = (await _weekmenuRepository.FindByConditionAsync(wm => wm.Week >=startDate&&wm.Week<=endDate)).ToList();

        // 准备存放菜单信息的列表
        var menuList = new List<object>();

        // 遍历每个 WeekMenu 项，仅选择 DisPrice 大于 0 的项
        foreach (var weekMenu in weekMenus.Where(wm => wm.DisPrice > 0))
        {
            // 使用 DishId 从 DishRepository 中获取菜品信息
            var dish = await _dishRepository.GetByIdAsync(weekMenu.DishId);

            if (dish != null)
            {
                // 将菜品信息映射到 Menu DTO 中
                menuList.Add(new
                {
                    DishId = dish.DishId,
                    DishName = dish.DishName,
                    Price = dish.Price,
                    DisPrice = weekMenu.DisPrice,
                    Picture = dish.ImageUrl,
                    Sales = weekMenu.Sales,
                });
            }
        }

        var result = new
        {
            success = menuList.Any(),
            msg = menuList.Any() ? "成功获取菜单" : "今日无促销菜单！",
            response = menuList,
        };

        return result;
    }
    public async Task<object> GetReviewsAsync()
    {
        // 从数据库中查询评价数据，并仅选择需要的字段
        var reviews = await (from review in _orderReviewRepository.GetAll()
                                 select new
                                 {
                                     OrderId = review.OrderId,
                                     cStars = review.CStars ?? (decimal?)null, // 如果 CStars 是 null，则保持 null
                                     cReviewText = review.CReviewText ?? "无评论" // 如果 CReviewText 是 null，提供默认文本
                                 }).ToListAsync();

        // 构造返回对象
        return new
        {
            success = true,
            msg = "成功获取食堂评价",
            response = reviews
        };
    }

}

