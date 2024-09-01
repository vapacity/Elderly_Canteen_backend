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
            //检查重名
            var existedName = await _dishRepository.FindByConditionAsync(e => e.DishName == dto.Name);
            if (existedName.Any())
            {
                return new DishResponseDto
                {
                    Success = false,
                    Msg = "name already existed",
                };
            }
            //检查食材存在性
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
            //检查类别存在性
            var existedCate = await _categoryRepository.GetByIdAsync(dto.CateId);
            if (existedCate == null) {
                return new DishResponseDto
                {
                    Success = false,
                    Msg = "cateId" + dto.CateId + "not found"
                };
            }
            //新dish
            var newDishId = await GenerateNewDishIdAsync();
            var newDish = new Dish
            {
                DishId = newDishId,
                DishName = dto.Name,
                Price = dto.Price,
                CateId = dto.CateId,
            };
            await _dishRepository.AddAsync(newDish);

            //新formula
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
            //获得ingredientname
            var formulaDtos = new List<FormulaDto>();
            foreach (var form in dto.Formula)
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(form.IngredientId);
                formulaDtos.Add(new FormulaDto
                {
                    IngredientId = form.IngredientId,
                    Amount = form.Amount,
                    IngredientName = ingredient?.IngredientName // 通过 ingredient 对象获取 IngredientName
                });
            }
            //返回
            return new DishResponseDto
            {
                Success = true,
                Msg = "success",
                Dish = new DishDto
                {
                    DishId = newDishId,
                    DishName = dto.Name,
                    Price = dto.Price,
                    Formula = formulaDtos // 将包含 ingredientName 的 FormulaDto 列表返回
                }
            };
        }

        public async Task<DishResponseDto> UpdateDish(DishRequestDto dto)
        {
            // 检查菜品是否存在
            var existingDish = await _dishRepository.GetByIdAsync(dto.DishId);
            if (existingDish == null)
            {
                return new DishResponseDto
                {
                    Success = false,
                    Msg = "DishId " + dto.DishId + " not found"
                };
            }

            // 检查是否有重名（排除当前菜品）
            var existedName = await _dishRepository.FindByConditionAsync(e => e.DishName == dto.Name && e.DishId != dto.DishId);
            if (existedName.Any())
            {
                return new DishResponseDto
                {
                    Success = false,
                    Msg = "name already existed"
                };
            }

            // 检查食材是否存在
            foreach (var formula in dto.Formula)
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(formula.IngredientId);
                if (ingredient == null)
                {
                    return new DishResponseDto
                    {
                        Success = false,
                        Msg = "ingredientId " + formula.IngredientId + " not found"
                    };
                }
            }

            // 检查类别是否存在
            var existedCate = await _categoryRepository.GetByIdAsync(dto.CateId);
            if (existedCate == null)
            {
                return new DishResponseDto
                {
                    Success = false,
                    Msg = "cateId " + dto.CateId + " not found"
                };
            }

            // 更新 Dish 信息
            existingDish.DishName = dto.Name;
            existingDish.Price = dto.Price;
            existingDish.CateId = dto.CateId;
            await _dishRepository.UpdateAsync(existingDish);

            // 删除原有的 Formula 记录
            var existingFormulas = await _formulaRepository.FindByConditionAsync(f => f.DishId == dto.DishId);
            foreach (var formula in existingFormulas)
            {
                await _formulaRepository.DeleteByCompositeKeyAsync<Formula>(formula.DishId,formula.IngredientId);
            }

            // 插入新的 Formula 记录
            foreach (var form in dto.Formula)
            {
                var newFormula = new Formula
                {
                    DishId = dto.DishId,
                    IngredientId = form.IngredientId,
                    Amount = form.Amount
                };
                await _formulaRepository.AddAsync(newFormula);
            }

            // 构建返回的 FormulaDto 列表，包含 ingredientName
            var formulaDtos = new List<FormulaDto>();
            foreach (var form in dto.Formula)
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(form.IngredientId);
                formulaDtos.Add(new FormulaDto
                {
                    IngredientId = form.IngredientId,
                    Amount = form.Amount,
                    IngredientName = ingredient?.IngredientName // 通过 ingredient 对象获取 IngredientName
                });
            }

            // 返回更新后的 DishResponseDto
            return new DishResponseDto
            {
                Success = true,
                Msg = "Dish updated successfully",
                Dish = new DishDto
                {
                    DishId = dto.DishId,
                    DishName = dto.Name,
                    Price = dto.Price,
                    Formula = formulaDtos
                }
            };
        }

        public async Task<DishResponseDto> DeleteDish(string dishId)
        {
            // 检查菜品是否存在
            var existingDish = await _dishRepository.GetByIdAsync(dishId);
            if (existingDish == null)
            {
                return new DishResponseDto
                {
                    Success = false,
                    Msg = "DishId " + dishId + " not found"
                };
            }

            // 删除与 DishId 相关的 Formula 记录
            await DeleteByDishIdAsync(dishId);

            // 删除 Dish 记录
            await _dishRepository.DeleteAsync(dishId);

            // 返回成功消息
            return new DishResponseDto
            {
                Success = true,
                Msg = "Dish and related formulas deleted successfully"
            };
        }

        private async Task DeleteByDishIdAsync(string dishId)
        {
            // 查找所有与 DishId 匹配的 Formula 记录
            var formulasToDelete = await _formulaRepository.FindByConditionAsync(f => f.DishId == dishId);

            // 删除每个找到的 Formula 记录
            foreach (var formula in formulasToDelete)
            {
                await _formulaRepository.DeleteByCompositeKeyAsync<Formula>(formula.DishId, formula.IngredientId);
            }
        }

        public async Task<AllDishResponseDto> SearchDishesAsync(string name)
        {
            List<Dish> dishes;

            if (string.IsNullOrWhiteSpace(name))
            {
                // 如果 name 为空或仅包含空白字符，返回所有菜品
                dishes = (await _dishRepository.GetAllAsync()).ToList();
            }
            else
            {
                // 如果 name 不为空，根据 dishName、ingredientName 和 categoryName 进行模糊搜索
                dishes = (await _dishRepository.FindByConditionAsync(d =>
                    d.DishName.Contains(name) ||
                    d.Formulas.Any(f => f.Ingredient.IngredientName.Contains(name)) ||
                    d.Cate.CateName.Contains(name)
                )).ToList();
            }

            foreach (var dish in dishes)
            {
                dish.Formulas = (await _formulaRepository.FindByConditionAsync(f => f.DishId == dish.DishId)).ToList();
                foreach (var formula in dish.Formulas)
                {
                    formula.Ingredient = await _ingredientRepository.GetByIdAsync(formula.IngredientId);
                }
            }

            // 构建返回的 AllDishResponseDto
            var dishDtos = new List<DishDto>();

            foreach (var dish in dishes)
            {
                // 查找与菜品关联的 Formula
                var formulaDtos = dish.Formulas.Select(f => new FormulaDto
                {
                    IngredientId = f.IngredientId,
                    Amount = f.Amount,
                    IngredientName = f.Ingredient?.IngredientName
                }).ToList();

                // 构建 DishDto
                var dishDto = new DishDto
                {
                    DishId = dish.DishId,
                    DishName = dish.DishName,
                    Price = dish.Price,
                    Formula = formulaDtos
                };

                dishDtos.Add(dishDto);
            }

            // 返回 AllDishResponseDto
            return new AllDishResponseDto
            {
                Dish = dishDtos,
                Msg = "success",
                Success = true
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
