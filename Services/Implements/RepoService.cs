using Elderly_Canteen.Data.Dtos.Ingredient;
using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace Elderly_Canteen.Services.Implements
{
    public class RepoService : IRepoService
    {
        private readonly IGenericRepository<Repository> _repoRepository;
        private readonly IGenericRepository<Ingredient> _ingreRepository;
        private readonly IGenericRepository<Finance> _financeRepository;
        private readonly IGenericRepository<Restock> _restockRepository;

        public RepoService(IGenericRepository<Repository> repoRepository, 
            IGenericRepository<Ingredient> ingreRepository, 
            IGenericRepository<Finance> financeRepository,
            IGenericRepository<Restock> restockRepository)
        {
            _repoRepository = repoRepository;
            _ingreRepository = ingreRepository;
            _financeRepository = financeRepository;
            _restockRepository = restockRepository;
        }

        public async Task<AllRepoResponseDto> GetRepo()
        {
            try
            {
                // 获取所有 Repository 和 Ingredient 数据
                var repositories = await _repoRepository.GetAllAsync();
                var ingredients = await _ingreRepository.GetAllAsync();

                // 执行连接查询
                var query = from repo in repositories
                            join ing in ingredients
                            on repo.IngredientId equals ing.IngredientId
                            select new RepoDto
                            {
                                Amount = repo.RemainAmount, // 假设 repo 中有 RemainAmount 字段
                                Expiry = repo.ExpirationTime, // 将 DateTime 转为 double
                                Grade = repo.HighConsumption, // 假设 HighConsumption 是 byte 类型，转换为 string
                                IngredientId = ing.IngredientId,
                                IngredientName = ing.IngredientName
                            };

                var ingredientsList = query.ToList();

                return new AllRepoResponseDto
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

                return new AllRepoResponseDto
                {
                    Ingredients = new List<RepoDto>(),
                    Message = "An error occurred while retrieving data.",
                    Success = false
                };
            }
        }

        /*public async Task<IngreResponseDto> AddRepo(RepoRequestDto dto)
        {
            string ingreName = dto.IngredientName;
            string ingreId = dto.IngredientId;
            int account = dto.Account;
            byte grade = dto.Grade;
            DateTime expiry = dto.newExpiry;
            //检查id是否有重复
            var existedRepo = await _repoRepository.FindByConditionAsync(e => e.IngredientId == ingreId);
            if (existedRepo.Any())
            {
                return new IngreResponseDto
                {
                    message = "ingredient id already existed!",
                    success = false
                };
            }

            //检查名字是否有重复
            var existedIngre = await _ingreRepository.FindByConditionAsync(e => e.IngredientName == ingreName);
            if (existedIngre.Any())
            {
                return new IngreResponseDto
                {
                    message = "ingredient name already existed!",
                    success = false
                };
            }

            //创建新ingre
            var ingre = new Ingredient
            {
                IngredientId = ingreId,
                IngredientName = ingreName,
            };
            await _ingreRepository.AddAsync(ingre);

            //创建新repo
            var repo = new Repository
            {
                IngredientId = ingreId,
                RemainAmount = account,
                HighConsumption = grade,
                ExpirationTime = expiry,
            };
            await _repoRepository.AddAsync(repo);


            return new IngreResponseDto
            {
                message = "Repo and Ingredient added successfully",
                success = true,
            };
        }
*/

        public async Task<RepoResponseDto> UpdateRepo(RepoRequestDto dto)
        {
            string ingreId = dto.IngredientId;
            string ingreName = dto.IngredientName;
            int amount = dto.Amount;
            byte grade = dto.Grade;
            DateTime? oldExpiry = dto.OldExpiry;
            DateTime newExpiry = dto.NewExpiry; // 原来的 ExpirationTime

            if (oldExpiry == null)
            {
                return new RepoResponseDto
                {
                    message = "oldExpiry not found",
                    success = false
                };
            }
            // 查找要更新的 Ingredient 实体
            var existedIngre = await _ingreRepository.GetByIdAsync(ingreId);
            if (existedIngre == null)
            {
                return new RepoResponseDto
                {
                    message = "ingredientId not found",
                    success = false
                };
            }

            // 查找要删除的旧 Repository 实体
            var existedRepo = await _repoRepository.FindByCompositeKeyAsync<Repository>(ingreId, oldExpiry);
            if (existedRepo == null)
            {
                return new RepoResponseDto
                {
                    message = "Old Repository record not found",
                    success = false
                };
            }

            // 删除旧的 Repository 实体
            await _repoRepository.DeleteByCompositeKeyAsync<Repository>(ingreId, oldExpiry);


            // 创建新的 Repository 实体
            var newRepo = new Repository
            {
                IngredientId = ingreId,
                ExpirationTime = newExpiry,
                HighConsumption = grade,
                RemainAmount = amount
            };

            await _repoRepository.AddAsync(newRepo);

            // 更新 Ingredient 实体
            existedIngre.IngredientName = ingreName;
            await _ingreRepository.UpdateAsync(existedIngre);

            return new RepoResponseDto
            {
                message = "Update successfully",
                success = true,
            };
        }

        public async Task<RepoResponseDto?> DeleteRepo(string ingreId, DateTime expiry)
        {
            var existedIngre = await _ingreRepository.GetByIdAsync(ingreId);
            var existedRepo = await _repoRepository.FindByCompositeKeyAsync<Repository>(ingreId, expiry);
            if (existedRepo == null || existedIngre == null)
                return new RepoResponseDto
                {
                    message = "not existed",
                    success = false,
                };
            else
            {
                await _repoRepository.DeleteByCompositeKeyAsync<Repository>(ingreId, expiry);
                await _ingreRepository.DeleteAsync(ingreId);
                return new RepoResponseDto
                {
                    message = "delete successfully",
                    success = true,
                };
            }
        }

        public async Task<RestockResponseDto> Restock(RestockRequestDto dto, string adminId)
        {
            string ingredientId = dto.IngredientId;
            int amount = dto.Amount;
            DateTime expiry = dto.Expiry;
            decimal price = dto.Price;

            try
            {
                // 获取食材名称
                var ingredientName = await _ingreRepository.GetByIdAsync(ingredientId);
                var existedRepo = await _repoRepository.FindByCompositeKeyAsync<Repository>(ingredientId, expiry);
                if (existedRepo !=null)
                {
                    existedRepo.RemainAmount += amount;
                    await _repoRepository.UpdateAsync(existedRepo);
                }
                else
                {
                    // 添加新的仓库记录
                    var newRepo = new Repository
                    {
                        IngredientId = ingredientId,
                        RemainAmount = amount,
                        HighConsumption = 1,
                        ExpirationTime = expiry,
                    };
                    await _repoRepository.AddAsync(newRepo);
                }
               
                // 生成财务记录ID
                string finId = await GenerateFinanceIdAsync();

                // 添加财务记录
                var newfin = new Finance
                {
                    FinanceId = finId,
                    FinanceType = "进货",
                    FinanceDate = DateTime.Now,
                    Price = price,
                    InOrOut = "1", // 0 in, 1 out
                    AccountId = adminId,
                    Status = "待审核"
                };
                await _financeRepository.AddAsync(newfin);

                // 添加进货记录
                var newRestock = new Restock
                {
                    FinanceId = finId,
                    IngredientId = ingredientId,
                    AdministratorId = adminId,
                    Quantity = amount,
                    Price = price,
                };
                await _restockRepository.AddAsync(newRestock);

                // 构建响应的数据部分
                var responseData = new RestockData
                {
                    Amount = amount,
                    Expiry = expiry,
                    FinanceId = finId,
                    IngredientId = ingredientId,
                    IngredientName = ingredientName?.IngredientName, // 假设 `GetByIdAsync` 返回的是 `Ingredient` 实体
                    Price = price,
                    Grade = 1 // 假设你需要一个 Grade 值，这里用 1 作为示例
                };

                // 返回成功的响应
                return new RestockResponseDto
                {
                    Success = true,
                    Message = "Restock successfully",
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                // 处理异常，记录日志或采取其他措施
                return new RestockResponseDto
                {
                    Success = false,
                    Message = $"Failed to restock: {ex.Message}",
                    Data = null
                };
            }
        }


        public async Task<List<RestockResponseDto>> GetRestockHistory()
        {
            return null;
        }

        //生成财务ID
        private async Task<string> GenerateFinanceIdAsync()
        {
            const string prefix = "200";

            var maxFinanceId = await _financeRepository.GetAll()
                .Where(f => f.FinanceId.StartsWith(prefix))
                .OrderByDescending(f => f.FinanceId)
                .Select(f => f.FinanceId)
                .FirstOrDefaultAsync();

            if (maxFinanceId == null)
            {
                return prefix + "00001";
            }

            // 提取数字部分并加1
            var numericalPart = int.Parse(maxFinanceId.Substring(prefix.Length));
            var newFinanceId = numericalPart + 1;

            // 使用前导零格式化新的Finance ID
            return prefix + newFinanceId.ToString("D5");
        }
    }
}
