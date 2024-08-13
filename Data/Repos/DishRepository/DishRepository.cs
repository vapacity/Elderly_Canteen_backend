using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Data.Repos.DishRepository
{
    public class DishRepository : IDishRepository
    {
        private IGenericRepository<Dish> _dishRepository { get; }
        private readonly IGenericRepository<Category> _cateRepository;
        private readonly IGenericRepository<Formula> _formulaRepository;

        public DishRepository(IGenericRepository<Dish> dishRepository,
                                   IGenericRepository<Formula> formulaRepository,
                                   IGenericRepository<Category> cateRepository)
        {
            _dishRepository = dishRepository;
            _formulaRepository = formulaRepository;
            _cateRepository = cateRepository;
        }

        public async Task<IEnumerable<Dish>> GetFormulaOfDishesAsync()
        {
            var dishes = await _dishRepository.GetAllAsync();
            var formulas = await _formulaRepository.GetAllAsync();

            // 结合数据进行处理
            // 假设要在这里做一些业务逻辑，比如将菜单信息添加到每个菜品中

            foreach (var dish in dishes)
            {
                // 在此处理每个 dish 和菜单之间的逻辑
            }

            return dishes;
        }
    }
}
