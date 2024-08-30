using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace Elderly_Canteen.Services.Implements
{
    public class RepoService:IRepoService
    {
        private readonly IGenericRepository<Repository> _repoRepository;
        private readonly IGenericRepository<Ingredient> _ingreRepository;
        
        public RepoService(IGenericRepository<Repository> repoRepository, IGenericRepository<Ingredient> ingreRepository)
        {
            _repoRepository = repoRepository;
            _ingreRepository = ingreRepository;
        }

        public async Task<AllResponseDto> GetRepo()
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
                            select new IngredientDto
                            {
                                Account = repo.RemainAmount, // 假设 repo 中有 RemainAmount 字段
                                Expiry = repo.ExpirationTime, // 将 DateTime 转为 double
                                Grade = repo.HighConsumption, // 假设 HighConsumption 是 byte 类型，转换为 string
                                IngredientId = ing.IngredientId,
                                IngredientName = ing.IngredientName
                            };

                var ingredientsList = query.ToList();

                return new AllResponseDto
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

                return new AllResponseDto
                {
                    Ingredients = new List<IngredientDto>(),
                    Message = "An error occurred while retrieving data.",
                    Success = false
                };
            }
        }


        public async Task<RepoResponseDto> AddIngredient(IngreRequestDto dto)
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
                return new RepoResponseDto
                {
                    message = "ingredient id already existed!",
                    success = false
                };
            }

            //检查名字是否有重复
            var existedIngre = await _ingreRepository.FindByConditionAsync(e => e.IngredientName == ingreName);
            if (existedIngre.Any())
            {
                return new RepoResponseDto
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

            
            return new RepoResponseDto
            {
                message = "Repo and Ingredient added successfully",
                success = true,
            };
        }

        public async Task<RepoResponseDto> UpdateIngredient(IngreRequestDto dto,string id)
        {
            string ingreId = id;
            string ingreName = dto.IngredientName;
            int account = dto.Account;
            byte grade = dto.Grade;
            DateTime? oldExpiry = dto.oldExpiry;
            DateTime newExpiry = dto.newExpiry; // 原来的 ExpirationTime

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
            var existedRepo = await _repoRepository.FindByCompositeKeyAsync<Repository>(ingreId,oldExpiry);
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
                RemainAmount = account
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

        public async Task<RepoResponseDto?> DeleteIngredient(string ingreId,DateTime expiry)
        {
            var existedIngre = await _ingreRepository.GetByIdAsync(ingreId);
            var existedRepo = await _repoRepository.FindByCompositeKeyAsync<Repository>(ingreId,expiry);
            if (existedRepo == null || existedIngre == null)
                return new RepoResponseDto
                {
                    message = "not existed",
                    success = false,
                };
            else
            {
                await _repoRepository.DeleteByCompositeKeyAsync<Repository>(ingreId,expiry);
                await _ingreRepository.DeleteAsync(ingreId);
                return new RepoResponseDto
                {
                    message = "delete successfully",
                    success = true,
                };
            }
        }
        
        public async Task<RepoResponseDto> Restock(string id)
        {
            return null;
        }


    }
}
