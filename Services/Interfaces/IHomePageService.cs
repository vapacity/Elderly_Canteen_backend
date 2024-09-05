using Elderly_Canteen.Data.Dtos.Dish;

public interface IHomePageService
{
    /// <summary>
    /// 获取菜品列表
    /// </summary>
    /// <returns></returns>
    Task<List<object>> GetDishesAsync();

    /// <summary>
    /// 获取菜品促销列表
    /// </summary>
    /// <returns></returns>
    Task<List<object>> GetThisWeekMenu();
    /// <summary>
    /// 获取今日促销列表
    /// </summary>
    /// <returns></returns>
    Task<dynamic> GetThisDayDiscountMenu();
    /// <summary>
    /// 获取评价
    /// </summary>
    /// <returns></returns>
    Task<object> GetReviewsAsync();

}
