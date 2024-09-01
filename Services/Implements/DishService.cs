using Elderly_Canteen.Data.Dtos.Dish;
using Microsoft.EntityFrameworkCore;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;

namespace Elderly_Canteen.Services.Implements
{
    public class DishService : IDishService
    {
        private readonly IGenericRepository<Dish> _dishRepository;
        private readonly IGenericRepository<Formula> _formulaRepository;
        private readonly IGenericRepository<Ingredient> _ingredientRepository;
        private readonly IGenericRepository<Category> _categoryRepository;

        public DishService(IGenericRepository<Dish> dishRepository,
                           IGenericRepository<Formula> formulaRepository,
                           IGenericRepository<Ingredient> ingredientRepository,
                           IGenericRepository<Category> categoryRepository)
        {
            _dishRepository = dishRepository;
            _formulaRepository = formulaRepository;
            _categoryRepository = categoryRepository;
            _ingredientRepository = ingredientRepository;
        }

        public async Task<DishResponseDto> AddDish(DishRequestDto dto)
        {
            var existedName = await _dishRepository.FindByConditionAsync(e => e.DishName == dto.Name);
            if (existedName.Any())
            {
                return new DishResponseDto
                {
                    Success = false,
                    Msg = "name already existed",
                };
            }
            foreach(var formula in dto.Formula)
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(formula.IngredientId);
                if(ingredient == null)
                {
                    return new DishResponseDto
                    {
                        Success = false,
                        Msg = "ingredientId"+formula.IngredientId+"not found"
                    };
                }
            }
            var existedCate = await _categoryRepository.GetByIdAsync(dto.CateId);
            if (existedCate == null) {
                return new DishResponseDto
                {
                    Success = false,
                    Msg = "cateId" + dto.CateId + "not found"
                };
            }
            var newDishId = await GenerateNewDishIdAsync();
            var newDish = new Dish
            {
                DishId = newDishId,
                DishName = dto.Name,
                Price = dto.Price,
                CateId = dto.CateId,
            };
            await _dishRepository.AddAsync(newDish);

            foreach(var form in dto.Formula)
            {
                var newFormula = new Formula
                {
                    DishId = newDishId,
                    IngredientId = form.IngredientId,
                    Amount = form.Amount
                };
                await _formulaRepository.AddAsync(newFormula);
            }

            return new DishResponseDto
            {
                Success = true,
                Msg = "success",
                Dish = new DishDto
                {
                    DishId = newDishId,
                    DishName= dto.Name,
                    Price = dto.Price,
                    Formula = dto.Formula.Select(f => new FormulaDto
                    {
                        IngredientId = f.IngredientId,
                        Amount = f.Amount
                    }).ToList()
                }
            };
        }


        private async Task<string> GenerateNewDishIdAsync()
        {
            // 获取数据库中当前 ingredient 表的最大 ID
            var maxId = await _dishRepository.GetAll()
                .OrderByDescending(e => e.DishId)
                .Select(e => e.DishId)
                .FirstOrDefaultAsync();

            // 如果没有记录，返回 "1"
            if (maxId == null)
            {
                return "1";
            }

            // 提取数字部分，并加 1
            int numericPart = int.Parse(maxId);
            int newNumericPart = numericPart + 1;

            // 返回新的 ID，作为字符串
            return newNumericPart.ToString();
        }
    }
}
