using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Data.Repos.DishRepository
{
    public interface IDishRepository
    {
        Task<IEnumerable<Dish>> GetFormulaOfDishesAsync();
    }
}
