using Elderly_Canteen.Data.Dtos.Dish;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using System.Text;


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
        // 加载默认静态图片的 byte[] 数据
        //byte[] defaultPicture = await LoadDefaultPictureAsync();

        // 从数据库中查询数据
        var result = await (from dish in _dishRepository.GetAll()
                            join weekmenu in _weekmenuRepository.GetAll()
                            on dish.DishId equals weekmenu.DishId
                            select new
                            {
                                dishName = dish.DishName,
                                picture =  dish.ImageUrl,
                                category = dish.Cate.CateName,
                                sale = weekmenu.Sales,
                                price = weekmenu.DisPrice > 0 ? weekmenu.DisPrice : dish.Price
                            }).ToListAsync();

        var finalResult = result.ToList<object>();

        return finalResult;
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

