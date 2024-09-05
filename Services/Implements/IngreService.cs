using Elderly_Canteen.Data.Dtos.Ingredient;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Elderly_Canteen.Services.Implements
{
    public class IngreService:IIngreService
    {
        private readonly IGenericRepository<Ingredient> _ingreRepository;
        private readonly IGenericRepository<Formula> _formulaRepository;
        
        public IngreService(IGenericRepository<Repository> repoRepository, IGenericRepository<Ingredient> ingreRepository,IGenericRepository<Formula> formulaRepository)
        {
            _ingreRepository = ingreRepository;
            _formulaRepository = formulaRepository;
        }

        public async Task<AllIngreResponseDto> GetRepo(string? name)
        {
            try
            {
                IEnumerable<Ingredient> ingredients;

                if (string.IsNullOrWhiteSpace(name))
                {
                    // 如果 name 为空或仅包含空白字符，返回所有食材
                    ingredients = await _ingreRepository.GetAllAsync();
                }
                else
                {
                    // 如果 name 不为空，按名称过滤
                    ingredients = await _ingreRepository.FindByConditionAsync(ing => ing.IngredientName.Contains(name));
                }

                // 将 ingredients 列表转换为 IngredientDto 列表
                var ingredientsList = ingredients
                    .Select(ing => new IngredientDto
                    {
                        IngredientId = ing.IngredientId,
                        IngredientName = ing.IngredientName,
                    })
                    .ToList();

                return new AllIngreResponseDto
                {
                    Ingredients = ingredientsList,
                    Message = "Data retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                // 记录异常（可选）
                Console.WriteLine($"An error occurred: {ex.Message}");

                return new AllIngreResponseDto
                {
                    Ingredients = null,
                    Message = "An error occurred while retrieving data.",
                    Success = false
                };
            }
        }


        public async Task<IngreResponseDto> AddIngredient(IngreRequestDto dto)
        {
            string ingreName = dto.IngredientName;
            //检查id是否有重复
            var existedName = await _ingreRepository.FindByConditionAsync(e=>e.IngredientName == ingreName);
            if(existedName.Any())
            {
                return new IngreResponseDto
                {
                    message = "ingredient name already existed!",
                    success = false
                };
            }
            string newId = await GenerateNewIdAsync();
            var existingAccount = await _ingreRepository.GetAll()
                .FirstOrDefaultAsync(a => a.IngredientId == newId);

            if (existingAccount != null)
            {
                return new IngreResponseDto
                {
                    message = "IngredientId already existed",
                    success = false,
                    IngredientId = newId,
                    IngredientName = dto.IngredientName
                };
            }
            //创建新ingre
            var ingre = new Ingredient
            {
                IngredientId = newId,
                IngredientName = ingreName,
            };
            await _ingreRepository.AddAsync(ingre);
           
            return new IngreResponseDto
            {
                message = "Ingredient added successfully",
                success = true,
                IngredientId = ingre.IngredientId,
                IngredientName = ingre.IngredientName,
            };
        }

        public async Task<IngreResponseDto> UpdateIngredient(IngreRequestDto dto)
        {
            string ingreId = dto.IngredientId;
            string ingreName = dto.IngredientName;

            // 查找要更新的 Ingredient 实体
            var existedIngre = await _ingreRepository.GetByIdAsync(ingreId);
            if (existedIngre == null)
            {
                return new IngreResponseDto
                {
                    message = "ingredientId not found",
                    success = false
                };
            }

            // 更新 Ingredient 实体
            existedIngre.IngredientName = ingreName;
            await _ingreRepository.UpdateAsync(existedIngre);

            return new IngreResponseDto
            {
                message = "Update successfully",
                success = true,
            };
        }

        public async Task<IngreResponseDto?> DeleteIngredient(string ingreId)
        {
            var existedIngre = await _ingreRepository.GetByIdAsync(ingreId);
            if (existedIngre == null)
                return new IngreResponseDto
                {
                    message = "not existed",
                    success = false,
                };
            else
            {
                var inFormula = await _formulaRepository.FindByConditionAsync(f=> f.IngredientId == ingreId);
                if (inFormula.Any())
                {
                    return new IngreResponseDto
                    {
                        message = "ingredient found in formula, delete refused",
                        success = false,
                    };
                }
                await _ingreRepository.DeleteAsync(ingreId);
                return new IngreResponseDto
                {
                    message = "delete successfully",
                    success = true,
                };
            }
        }
        private async Task<string> GenerateNewIdAsync()
        {
            var maxId = (await _ingreRepository.GetAll()
                .ToListAsync()) // 将查询结果加载到内存
                .OrderByDescending(e => int.TryParse(e.IngredientId, out int id) ? id : 0) // 转换为整数并进行排序
                .Select(e => e.IngredientId)
                .FirstOrDefault();

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
