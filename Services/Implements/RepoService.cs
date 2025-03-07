using Elderly_Canteen.Data.Dtos.Ingredient;
using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Data;

namespace Elderly_Canteen.Services.Implements
{
    public class RepoService : IRepoService
    {
        private readonly IGenericRepository<Repository> _repoRepository;
        private readonly IGenericRepository<Ingredient> _ingreRepository;
        private readonly IGenericRepository<Formula> _formulaRepository;
        private readonly IGenericRepository<Finance> _financeRepository;
        private readonly IGenericRepository<Restock> _restockRepository;
        private readonly IGenericRepository<Administrator> _administratorRepository;
        private readonly IGenericRepository<Weekmenu> _weekMenuRepository;
        private readonly IGenericRepository<Dish> _dishRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogService _logService;
        private readonly ModelContext _context;

        public RepoService(IGenericRepository<Repository> repoRepository, 
            IGenericRepository<Ingredient> ingreRepository, 
            IGenericRepository<Finance> financeRepository,
            IGenericRepository<Restock> restockRepository,
            IGenericRepository<Administrator> administratorRepository,
            IGenericRepository<Weekmenu> weekMenuRepository,
            IGenericRepository<Formula> formulaRepository,
            IGenericRepository<Dish> dishRepository,
            INotificationService notificationService,
            ILogService logService,
            ModelContext context)
        {
            _repoRepository = repoRepository;
            _ingreRepository = ingreRepository;
            _financeRepository = financeRepository;
            _restockRepository = restockRepository;
            _administratorRepository = administratorRepository;
            _weekMenuRepository = weekMenuRepository;
            _notificationService = notificationService;
            _formulaRepository = formulaRepository;
            _dishRepository = dishRepository;
            _context = context;
            _logService = logService;
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
                query = query.OrderBy(dto => dto.Expiry);
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
                //await _ingreRepository.DeleteAsync(ingreId);
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
                //数量不对
                if (amount <=0)
                {
                    return new RestockResponseDto
                    {
                        Success = false,
                        Message = "wrong amount.",
                        Data = null
                    };
                }
                //价格不对
                if (price < 0)
                {
                    return new RestockResponseDto
                    {
                        Success = false,
                        Message = "wrong price.",
                        Data = null
                    };
                }
                //有效期已到期
                if (expiry < DateTime.Now)
                {
                    return new RestockResponseDto
                    {
                        Success = false,
                        Message = "you can't restock ingredient already expire.",
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
                    Amount = r.Quantity,
                    Date = finances.TryGetValue(r.FinanceId, out var finance) ? finance.FinanceDate : DateTime.Now,
                    ExpirationTime = repositoryData.TryGetValue(r.IngredientId, out var repo) ? repo.ExpirationTime : DateTime.MinValue,
                    FinanceId = r.FinanceId,
                    IngredientId = r.IngredientId,
                    IngredientName = ingredients.TryGetValue(r.IngredientId, out var ingredient) ? ingredient.IngredientName : "Unknown",
                    Price = r.Price
                }).OrderByDescending(r=>r.Date).ToList();

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
        //计算当前周数
        private DateTime GetWeekStartDate()
        {
            DateTime today = DateTime.Now;
            DayOfWeek dayOfWeek = today.DayOfWeek;
            int daysToMonday = (int)dayOfWeek - (int)DayOfWeek.Monday;
            DateTime weekStartDate = today.AddDays(-daysToMonday).Date; // 获取本周的周一
            return weekStartDate;
        }
        // 映射 DayOfWeek 到 Mon, Tue, Wed 等字符串
        private string MapDayOfWeekToShortString(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Mon",
                DayOfWeek.Tuesday => "Tue",
                DayOfWeek.Wednesday => "Wed",
                DayOfWeek.Thursday => "Thu",
                DayOfWeek.Friday => "Fri",
                DayOfWeek.Saturday => "Sat",
                DayOfWeek.Sunday => "Sun",
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
            };
        }
        
        
        // 检查并减少库存逻辑
        public async Task<bool> CheckAndReduceStockAsync(string dishId, DateTime week, int quantity)
        {
            // 1. 将当前日期转换为简短字符串形式（如 "Mon"）
            string today = MapDayOfWeekToShortString(DateTime.Now.DayOfWeek);

            // 2. 获取本周开始的日期
            DateTime weekStar = week;

            // 3. 查找对应菜品在当前周和当天的库存记录
            var wkmu = (await _weekMenuRepository.FindByConditionAsync(w => w.DishId == dishId && w.Week == week.Date && w.Day == today)).FirstOrDefault();

            // 4. 如果未找到记录，表示库存不足或无效
            if (wkmu == null)
                return false;

            // 5. 获取当前库存
            int stock = wkmu.Stock;

            // 6. 如果库存不足以满足需求，返回 false
            if (stock < quantity) // 注意: 如果需求量与库存相等也应视为库存充足，可以完全消耗
            {
                return false;
            }
            var dish = await _dishRepository.GetByIdAsync(dishId);
            
            // 7. 库存充足，减少库存，这里需要使用锁以确保线程安全
            // 使用乐观锁或数据库事务来确保库存的准确性
            wkmu.Stock -= quantity;
            wkmu.Sales += quantity;
            // 8. 如果减少后的库存低于或等于10，触发低库存通知
            if (wkmu.Stock < 10 && wkmu.Stock>0)
            {
                // 调用通知服务发送低库存警告
                // 尚未实现
                await _logService.LogAsync("Warning", $"今日菜品 {dish.DishName} 库存不足10份,目前为{wkmu.Stock}份，请及时补货");

                
            }
            else if(wkmu.Stock == 0)
            {
                await _logService.LogAsync("Danger", $"今日菜品 {dish.DishName} 已售罄，请及时补货！！");
            }

            // 9. 更新库存信息
            await _weekMenuRepository.UpdateAsync(wkmu);

            return true;
        }

        // 每天自动更新库存
        public async Task ReplenishDailyStockAsync()
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. 获取当前周的周一日期
                    DateTime weekStartDate = GetWeekStartDate();
                    string today = MapDayOfWeekToShortString(DateTime.Now.DayOfWeek);

                    // 2. 获取所有当天的 WeekMenu 项目
                    var weekMenus = await _weekMenuRepository.FindByConditionAsync(w => w.Week == DateTime.Now.Date);

                    foreach (var weekMenu in weekMenus)
                    {
                        // 3. 检查是否有足够的食材库存来支持 50 份
                        int maxPortions = await CalculateMaxPortionsAsync(weekMenu.DishId, 50);
                        weekMenu.Sales = 0;
                        // 4. 如果库存不足，调整库存为能支持的最大份数
                        if (maxPortions < 50)
                        {
                            weekMenu.Stock = maxPortions;
                            var dish = await _dishRepository.GetByIdAsync(weekMenu.DishId);
                            if (maxPortions == 0)
                                await _logService.LogAsync("Dangerous", $"今日配料库存不足，{dish.DishName} 的库存为零");
                            else
                            {
                                await _logService.LogAsync("Warning", $"今日配料库存不足，{dish.DishName} 的库存减为 {weekMenu.Stock} 份");
                            }
                            Console.WriteLine($"库存不足，{weekMenu.DishId} 的库存调整为 {weekMenu.Stock} 份");
                        }
                        else
                        {
                            weekMenu.Stock = 50;
                            var dish = await _dishRepository.GetByIdAsync(weekMenu.DishId);
                            await _logService.LogAsync("Safe", $"{dish.DishName} 的库存已自动设置为 50 份");
                        }
                        await _weekMenuRepository.UpdateAsync(weekMenu);
                        bool success = await ReduceIngredientStockAsync(weekMenu.DishId, weekMenu.Stock);
                        if (!success)
                        {
                            throw new Exception($"库存减少失败，无法支持 {weekMenu.Stock} 份 {weekMenu.DishId}");
                        }
                    }

/*                    // 5. 更新 WeekMenu 中的库存
                    foreach (var weekMenu in weekMenus)
                    {
                        
                    }

                    // 6. 减少 repository 中的食材库存
                    foreach (var weekMenu in weekMenus)
                    {
                        
                    }*/

                    // 7. 提交事务
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // 发生异常时回滚事务
                    await transaction.RollbackAsync();
                    Console.WriteLine($"发生错误: {ex.Message}");
                    throw;
                }
            }
        }

        // 每天删除过期食材
        public async Task CheckAndRemoveExpiredIngredientsAsync()
        {
            
            await _repoRepository.DeleteByConditionAsync(r => r.ExpirationTime <= DateTime.Now || r.RemainAmount == 0);

            var ingredients = (await _repoRepository.GetAllAsync())
                              .OrderBy(r => r.ExpirationTime)
                              .ToList();
            foreach (var ingredient in ingredients)
            {
                // 删除过期食材
                if (ingredient.ExpirationTime <= DateTime.Now.AddDays(7))
                {
                    var ingredientName = await _ingreRepository.GetByIdAsync(ingredient.IngredientId);
                    await _logService.LogAsync("Warning", $"食材{ingredientName.IngredientName}存在即将过期部分");
                }
            }
        }

        // 计算当前仓库 能够制作的最大份数
        public async Task<int> CalculateMaxPortionsAsync(string dishId, int requestedQuantity = 50)
        {
            // 查询该菜品的配方信息
            var formulaItems = await _formulaRepository.FindByConditionAsync(f => f.DishId == dishId);

            // 初始化最大可制作份数为无限大（表示还未受任何食材限制）
            int maxPortions = int.MaxValue;

            // 遍历配方中的每种食材，计算每种食材能制作的最大份数
            foreach (var formula in formulaItems)
            {
                // 计算制作 requestedQuantity 份菜品所需的食材总量
                int requiredQuantity = requestedQuantity * formula.Amount;

                // 查询该食材的库存，并按保质期排序
                var ingredients = (await _repoRepository.FindByConditionAsync(r => r.IngredientId == formula.IngredientId))
                                   .OrderBy(r => r.ExpirationTime)
                                   .ToList();

                // 计算该食材的总库存量
                int sumAmount = ingredients.Sum(i => i.RemainAmount);

                // 如果库存不足，计算当前库存能制作的最大份数
                if (sumAmount < requiredQuantity)
                {
                    // 当前库存能支持的最大份数
                    int possiblePortions = sumAmount / formula.Amount;

                    // 更新最大可制作份数
                    maxPortions = Math.Min(maxPortions, possiblePortions);
                }
            }

            // 如果 maxPortions 被限制为一个较小的值，则更新 requestedQuantity
            if (maxPortions < requestedQuantity)
            {
                requestedQuantity = maxPortions;
                Console.WriteLine($"库存不足，最多可制作 {maxPortions} 份 {dishId}");
            }

            return maxPortions;
        }

        // 根据输入减少库存量
        public async Task<bool> ReduceIngredientStockAsync(string dishId, int requiredQuantity)
        {
            // 获取配方信息
            var formulaItems = await _formulaRepository.FindByConditionAsync(f => f.DishId == dishId);

            // 遍历配方中的每种食材，减少库存
            foreach (var formula in formulaItems)
            {
                int totalRequiredQuantity = requiredQuantity * formula.Amount;

                // 获取对应食材的库存，并按保质期排序
                var ingredients = (await _repoRepository.FindByConditionAsync(r => r.IngredientId == formula.IngredientId))
                                  .OrderBy(r => r.ExpirationTime)
                                  .ToList();

                foreach (var ingredient in ingredients)
                {
                    if (ingredient.RemainAmount >= totalRequiredQuantity)
                    {
                        ingredient.RemainAmount -= totalRequiredQuantity;
                        totalRequiredQuantity = 0;
                    }
                    else
                    {
                        totalRequiredQuantity -= ingredient.RemainAmount;
                        ingredient.RemainAmount = 0;
                    }

                    // 更新食材库存
                    await _repoRepository.UpdateAsync(ingredient);

                    // 如果食材已足够，无需继续减少
                    if (totalRequiredQuantity == 0)
                        break;
                }

                if (totalRequiredQuantity > 0)
                {
                    // 库存不足，无法满足需求
                    return false;
                }
            }

            // 库存成功减少
            return true;
        }

        public async Task<bool> RecalculateHighConsumption()
        {
            try
            {
                // 从 restock 表中获取所有记录
                var restocks = await _context.Restocks.ToListAsync();

                // 按 IngredientId 分组，计算每种食材的总进货数量
                var ingredientRestockTotals = restocks
                    .GroupBy(r => r.IngredientId)
                    .Select(group => new
                    {
                        IngredientId = group.Key,
                        TotalQuantity = group.Sum(r => r.Quantity)
                    })
                    .ToList();

                // 计算所有进货记录的总进货数量
                var totalQuantity = ingredientRestockTotals.Sum(ir => ir.TotalQuantity);

                // 遍历每个 IngredientId，计算高耗材百分比
                foreach (var ingredientRestock in ingredientRestockTotals)
                {
                    var percentage = (ingredientRestock.TotalQuantity / (decimal)totalQuantity) * 100;

                    byte highConsumptionValue;

                    if (percentage <= 20)
                        highConsumptionValue = 1;
                    else if (percentage <= 40)
                        highConsumptionValue = 2;
                    else if (percentage <= 60)
                        highConsumptionValue = 3;
                    else if (percentage <= 80)
                        highConsumptionValue = 4;
                    else
                        highConsumptionValue = 5;


                    // 使用 repository 查找对应的 Ingredient 并更新 highConsumption 值
                    var ingredientsToUpdate = await _repoRepository.FindByConditionAsync(i => i.IngredientId == ingredientRestock.IngredientId);

                    foreach (var ingredient in ingredientsToUpdate)
                    {
                        ingredient.HighConsumption = highConsumptionValue;
                        await _repoRepository.UpdateAsync(ingredient);
                    }
                    
                }

                // 保存所有更改
                await _context.SaveChangesAsync();
                await _logService.LogAsync("Safe", "已更新仓库中的高消耗属性");
                return true;
            }
            catch (Exception ex)
            {
                // 错误处理，可以记录日志或返回错误信息
                Console.WriteLine($"An error occurred while recalculating high consumption: {ex.Message}");
                return false;
            }
        }

    }

}

