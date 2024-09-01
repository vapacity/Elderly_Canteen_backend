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
        private readonly IGenericRepository<Administrator> _administratorRepository;
        public RepoService(IGenericRepository<Repository> repoRepository, 
            IGenericRepository<Ingredient> ingreRepository, 
            IGenericRepository<Finance> financeRepository,
            IGenericRepository<Restock> restockRepository,
            IGenericRepository<Administrator> administratorRepository)
        {
            _repoRepository = repoRepository;
            _ingreRepository = ingreRepository;
            _financeRepository = financeRepository;
            _restockRepository = restockRepository;
            _administratorRepository = administratorRepository;
        }

        public async Task<AllRepoResponseDto> GetRepo(string? name)
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
                                Amount = repo.RemainAmount,
                                Expiry = repo.ExpirationTime,
                                Grade = repo.HighConsumption,
                                IngredientId = ing.IngredientId,
                                IngredientName = ing.IngredientName
                            };

                // 如果提供了 name 参数，进行过滤
                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(dto => dto.IngredientName.Contains(name, StringComparison.OrdinalIgnoreCase));
                }

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
                // 记录异常
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
                if (ingredientName == null)
                {
                    // 处理食材不存在的情况
                    return new RestockResponseDto
                    {
                        Success = false,
                        Message = "Ingredient not found.",
                        Data = null
                    };
                }
                var existedRepo = await _repoRepository.FindByCompositeKeyAsync<Repository>(ingredientId, expiry);
                
                if (existedRepo !=null)
                {
                    await _repoRepository.UpdateAsync(
                        r => r.IngredientId == ingredientId && r.ExpirationTime == expiry,
                        r => r.RemainAmount += amount
                    );
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
                    AdministratorId = adminId,
                    Amount = amount,
                    Expiry = expiry,
                    FinanceId = finId,
                    IngredientId = ingredientId,
                    IngredientName = ingredientName?.IngredientName, // 假设 `GetByIdAsync` 返回的是 `Ingredient` 实体
                    Price = price,
                    Grade = 1, // 假设你需要一个 Grade 值，这里用 1 作为示例
                    Date = DateTime.Now,
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


        public async Task<AllRestockResponseDto> GetRestockHistory()
        {
            try
            {
                // 获取所有的 Restock 记录
                var restocks = await _restockRepository.GetAllAsync();

                // 如果没有记录，返回相应的响应
                if (restocks == null || !restocks.Any())
                {
                    return new AllRestockResponseDto
                    {
                        Success = true,
                        Message = "No restock history found.",
                        Restocks = new List<Restocks>()
                    };
                }

                // 收集所有需要查询的 ID
                var ingredientIds = restocks.Select(r => r.IngredientId).Distinct().ToList();
                var adminIds = restocks.Select(r => r.AdministratorId).Distinct().ToList();
                var financeIds = restocks.Select(r => r.FinanceId).Distinct().ToList();

                // 获取所有相关的食材信息
                var ingredients = new Dictionary<string, Ingredient>();
                foreach (var id in ingredientIds)
                {
                    var ingredient = await _ingreRepository.GetByIdAsync(id);
                    if (ingredient != null)
                    {
                        ingredients[id] = ingredient;
                    }
                }

                // 获取所有相关的管理员信息
                var admins = new Dictionary<string, Administrator>();
                foreach (var id in adminIds)
                {
                    var admin = await _administratorRepository.GetByIdAsync(id);
                    if (admin != null)
                    {
                        admins[id] = admin;
                    }
                }

                // 获取所有相关的财务信息
                var finances = new Dictionary<string, Finance>();
                foreach (var id in financeIds)
                {
                    var finance = await _financeRepository.GetByIdAsync(id);
                    if (finance != null)
                    {
                        finances[id] = finance;
                    }
                }

                // 获取相关的仓库记录信息
                var repositoryData = new Dictionary<string, Repository>();
                foreach (var id in ingredientIds)
                {
                    var repo = await _repoRepository.FindByCompositeKeyAsync<Repository>(id, DateTime.Now); // 使用合适的日期
                    if (repo != null)
                    {
                        repositoryData[id] = repo;
                    }
                }

                // 构建 Restock 数据列表
                var restockDataList = restocks.Select(r => new Restocks
                {
                    AdministratorId = r.AdministratorId,
                    AdministratorName = admins.TryGetValue(r.AdministratorId, out var admin) ? admin.Name : "Unknown",
                    Amount = r.Quantity,
                    Date = finances.TryGetValue(r.FinanceId, out var finance) ? finance.FinanceDate : DateTime.Now,
                    ExpirationTime = repositoryData.TryGetValue(r.IngredientId, out var repo) ? repo.ExpirationTime : DateTime.MinValue,
                    FinanceId = r.FinanceId,
                    IngredientId = r.IngredientId,
                    IngredientName = ingredients.TryGetValue(r.IngredientId, out var ingredient) ? ingredient.IngredientName : "Unknown",
                    Price = r.Price
                }).ToList();

                // 返回成功的响应
                return new AllRestockResponseDto
                {
                    Success = true,
                    Message = "Restock history retrieved successfully.",
                    Restocks = restockDataList
                };
            }
            catch (Exception ex)
            {
                return new AllRestockResponseDto
                {
                    Success = false,
                    Message = $"Failed to retrieve restock history: {ex.Message}",
                    Restocks = null
                };
            }
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
