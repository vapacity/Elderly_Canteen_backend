using Elderly_Canteen.Data.Dtos.Cart;
using Elderly_Canteen.Data.Dtos.Category;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Elderly_Canteen.Services.Implements
{
    public class CateService : ICateService
    {
        private readonly IGenericRepository<Category> _cateRepository;

        public CateService(IGenericRepository<Category> cateRepository)
        {
            _cateRepository = cateRepository;
        }

        public async Task<AllCateResponseDto> GetCate(string? name)
        {
            try
            {
                IQueryable<Category> categoriesQuery = _cateRepository.GetAll();

                if (!string.IsNullOrEmpty(name))
                {
                    categoriesQuery = categoriesQuery.Where(cat => cat.CateName.Contains(name));
                }

                var categories = await categoriesQuery.ToListAsync();

                var categoriesList = categories
                    .Select(cat => new Cate
                    {
                        CateId = cat.CateId,
                        CateName = cat.CateName,
                    })
                    .ToList();

                return new AllCateResponseDto
                {
                    Cates = categoriesList,
                    Message = "Data retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"An error occurred: {ex.Message}");

                return new AllCateResponseDto
                {
                    Cates = null,
                    Message = "An error occurred while retrieving data.",
                    Success = false
                };
            }
        }

        public async Task<CateResponseDto> AddCate(CateRequestDto dto)
        {
            string cateName = dto.CateName;

            var existedName = await _cateRepository.FindByConditionAsync(e => e.CateName == cateName);
            if (existedName.Any())
            {
                return new CateResponseDto
                {
                    Message = "Category name already existed!",
                    Success = false
                };
            }

            string newId = await GenerateNewIdAsync();

            var cate = new Category
            {
                CateId = newId,
                CateName = cateName,
            };

            await _cateRepository.AddAsync(cate);

            var newCate = new CateDto
            {
                CateId = newId,
                CateName = cateName,
            };
            return new CateResponseDto
            {
                Message = "Category added successfully",
                Success = true,
                Cates = newCate
            };
        }

        public async Task<CateResponseDto> UpdateCate(CateRequestDto dto)
        {
            string cateId = dto.CateId;
            string cateName = dto.CateName;

            var existedCate = await _cateRepository.GetByIdAsync(cateId);
            if (existedCate == null)
            {
                return new CateResponseDto
                {
                    Message = "CategoryId not found",
                    Success = false
                };
            }

            existedCate.CateName = cateName;
            await _cateRepository.UpdateAsync(existedCate);
            var newCate = new CateDto
            {
                CateId = existedCate.CateId,
                CateName = existedCate.CateName
            };
            return new CateResponseDto
            {
                Message = "Update successfully",
                Success = true,
                Cates = newCate
            };
        }

        public async Task<CateResponseDto?> DeleteCate(string cateId)
        {
            var existedCate = await _cateRepository.GetByIdAsync(cateId);
            if (existedCate == null)
            {
                return new CateResponseDto
                {
                    Message = "Category not existed",
                    Success = false,
                };
            }

            await _cateRepository.DeleteAsync(cateId);

            return new CateResponseDto
            {
                Message = "Delete successfully",
                Success = true,
            };
        }

        private async Task<string> GenerateNewIdAsync()
        {
            var maxId = await _cateRepository.GetAll()
                .OrderByDescending(e => e.CateId)
                .Select(e => e.CateId)
                .FirstOrDefaultAsync();

            if (maxId == null)
            {
                return "1";
            }

            int numericPart = int.Parse(maxId);
            int newNumericPart = numericPart + 1;

            return newNumericPart.ToString();
        }
    }
}
