using Elderly_Canteen.Data.Dtos.WeekMenu;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Elderly_Canteen.Services.Implements
{
    public class WeekMenuService:IWeekMenuService
    {
        private readonly IGenericRepository<Weekmenu> _weekMenuRepository;
        private readonly IGenericRepository<Dish> _dishRepository;
        private readonly IGenericRepository<Category> _cateRepository;
        private readonly IRepoService _repoService;
        public WeekMenuService(IGenericRepository<Weekmenu> weekMenuRepository,
                                IGenericRepository<Dish> dishRepository,
                                IGenericRepository<Category> cateRepository,
                                IRepoService repoService)
        {
            _weekMenuRepository = weekMenuRepository;
            _dishRepository = dishRepository;
            _cateRepository = cateRepository;
            _repoService = repoService;
        }
        public DateTime GetWeekStartDate(DateTime inputDate)
        {
            // 基准日期：2024-09-02（周一）
            DateTime baseDate = new DateTime(2024, 9, 2);

            // 计算输入日期与基准日期的天数差距
            int daysDifference = (inputDate.Date - baseDate.Date).Days;

            // 计算周数
            int weekNumber = daysDifference / 7 + 1;

            // 计算对应周的周一日期
            DateTime weekStartDate = baseDate.AddDays((weekNumber - 1) * 7);

            return weekStartDate;
        }
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
        public async Task<WmResponseDto> AddWM(WmRequestDto request)
        {
            var dishId = request.DishId;
            var day = MapDayOfWeekToShortString(request.Date.DayOfWeek);
            var existedDish = await _dishRepository.GetByIdAsync(dishId);
            if (existedDish == null)
            {
                return new WmResponseDto
                {
                    Success = false,
                    Message = "not found"
                };
            }

            var weekDate = GetWeekStartDate(request.Date);

            var existedWM = await _weekMenuRepository.FindByCompositeKeyAsync<Weekmenu>(dishId, request.Date.Date);
            if (existedWM != null)
            {
                return new WmResponseDto
                {
                    Success = false,
                    Message = "dish already existed today",
                    
                };
            }

            //var maxPortion =await  _repoService.CalculateMaxPortionsAsync(dishId);

            var newWM = new Weekmenu
            {
                DishId = dishId,
                Week = request.Date.Date,//改成今日天的日期，需写一个方法判断是否同周
                Stock = 50,//先写作默认
                Sales = 0,
                DisPrice = 0,
                Day = day,
            };
            await _weekMenuRepository.AddAsync(newWM);
            var category = await _cateRepository.GetByIdAsync(existedDish.CateId);
            return new WmResponseDto
            { 
                Success= true,
                Message = "success",
                Dish = new DishInfo
                {
                    Category = category.CateName,
                    DishName = existedDish.DishName
                }
            };


        }

        public async Task<AllWeekMenuResponseDto> GetWeekMenuByDateAsync(DateTime date)
        {
            // 计算当周的开始日期
            var weekStartDate = GetWeekStartDate(date);
            var weekEndDate = weekStartDate.AddDays(6);
            // 查询数据库中符合条件的所有条目
            var weekMenus = await _weekMenuRepository.FindByConditionAsync(wm =>
                wm.Week.Date >= weekStartDate && wm.Week.Date <= weekEndDate);

            if (!weekMenus.Any())
            {
                return null;
            }
            var response = new AllWeekMenuResponseDto
            {
                Mon = new List<Mon>(),
                Tue = new List<Tue>(),
                Wed = new List<Wed>(),
                Thu = new List<Thu>(),
                Fri = new List<Fri>(),
                Sat = new List<Sat>(),
                Sun = new List<Sun>()
            };

            foreach (var weekMenu in weekMenus)
            {
                // 根据 dishId 从 Dish 表中获取 Dish 实体
                var dish = await _dishRepository.GetByIdAsync(weekMenu.DishId);
                if (dish == null) continue; // 跳过未找到的 dish

                // 根据 cateId 从 Category 表中获取 Category 实体
                var category = await _cateRepository.GetByIdAsync(dish.CateId);
                var categoryName = category?.CateName; // 获取 categoryName

                var menuDto = new
                {
                    Category = categoryName,
                    Id = weekMenu.DishId,
                    Name = dish.DishName
                };

                // 根据 weekMenu.Day 的值将菜单项分类到对应的列表中
                switch (weekMenu.Day)
                {
                    case "Mon":
                        response.Mon.Add(new Mon { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Tue":
                        response.Tue.Add(new Tue { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Wed":
                        response.Wed.Add(new Wed { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Thu":
                        response.Thu.Add(new Thu { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Fri":
                        response.Fri.Add(new Fri { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Sat":
                        response.Sat.Add(new Sat { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Sun":
                        response.Sun.Add(new Sun { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                }
            }

            return response;
        }


        public async Task<Weekmenu> FindByCompositeAsync(string dishId, DateTime weekDate, string day)
        {
            // 使用 FindByConditionAsync 查找符合条件的实体
            var entities = await _weekMenuRepository.FindByConditionAsync(wm =>
                wm.DishId.Equals(dishId) &&
                wm.Week.Equals(weekDate) &&
                wm.Day.Equals(day)
            );

            // 返回找到的第一个实体（如果有的话）
            return entities.FirstOrDefault();
        }

        public async Task<bool> DeleteByCompositeAsync(string dishId, DateTime weekDate, string day)
        {
            return await _weekMenuRepository.DeleteByConditionAsync(wm =>
                wm.DishId.Equals(dishId) &&
                wm.Week.Equals(weekDate) &&
                wm.Day.Equals(day)
            );
        }

        public async Task<WmResponseDto> RemoveWM(WmRequestDto request)
        {
            var dishId = request.DishId;
            var day = MapDayOfWeekToShortString(request.Date.DayOfWeek);
            var existedDish = await _dishRepository.GetByIdAsync(dishId);
            if (existedDish == null)
            {
                return new WmResponseDto
                {
                    Success = false,
                    Message = "Dish not found"
                };
            }

            var weekStartDate = GetWeekStartDate(request.Date);
            var weekEndDate = weekStartDate.AddDays(6);

            var existedWM = await _weekMenuRepository.DeleteByConditionAsync(wm =>
                wm.Week.Date == request.Date.Date && wm.DishId == dishId);

            if (!existedWM)
            {
                return new WmResponseDto
                {
                    Success = false,
                    Message = "Dish does not exist in this week or in this day",
                };
            }

            return new WmResponseDto
            {
                Success = true,
                Message = "Successfully deleted"
            };
        }

        public async Task<DiscountResponseDto> UploadDiscount(DiscountRequestDto dto)
        {
            var weekStartDate = GetWeekStartDate(dto.Date).Date;
            var weekEndDate = weekStartDate.AddDays(6);
            var discount = dto.Discount;
            var dishId = dto.DishId;

            var existedDish = await _weekMenuRepository.FindByCompositeKeyAsync<Weekmenu>(dishId, dto.Date.Date);
            if(existedDish == null)
            {
                return new DiscountResponseDto
                {
                    Success = false,
                    Message = "dish not found today"
                };
            }
            var dishPrice = (await _dishRepository.GetByIdAsync(dishId)).Price;
            decimal disPrice = dishPrice * dto.Discount;
            existedDish.DisPrice = disPrice;
            await _weekMenuRepository.UpdateAsync(existedDish);
            return new DiscountResponseDto
            {
                Success = true,
                Message = "success",
                DishId = dishId,
                UpdatedPrice = disPrice
            };
        }

        public async Task<BatchResponseDto> BatchDiscount(BatchRequestDto dto)
        {
            var weekStartDate = GetWeekStartDate(dto.Date).Date;
            var weekEndDate = weekStartDate.AddDays(6);
            var discount = dto.Discount;
            var dishIds = dto.DishIds;

            var response = new BatchResponseDto
            {
                DiscountDishes = new List<DiscountDish>(),
                Success = true,
                Message = "Batch discount applied successfully."
            };

            foreach (var dishId in dishIds)
            {
                // 查找指定 dishId 的周菜单记录
                var existedDish = await _weekMenuRepository.FindByCompositeKeyAsync<Weekmenu>(dishId, dto.Date.Date);
                if (existedDish == null)
                {
                    response.Success = false;
                    response.Message = $"Dish with ID {dishId} not found in this week.";
                    continue;
                }

                // 获取菜品信息
                var dish = await _dishRepository.GetByIdAsync(dishId);
                if (dish == null)
                {
                    response.Success = false;
                    response.Message = $"Dish with ID {dishId} not found.";
                    continue;
                }

                // 计算折扣后的价格
                var discountPrice = dish.Price * discount;
                existedDish.DisPrice = discountPrice;

                // 更新周菜单记录
                await _weekMenuRepository.UpdateAsync(existedDish);

                // 添加到返回的结果中
                response.DiscountDishes.Add(new DiscountDish
                {
                    DishId = dishId,
                    DishName = dish.DishName,
                    DiscountPrice = discountPrice
                });
            }

            return response;
        }
        public async Task<AllDiscountResponseDto> GetAllDiscount(DateTime date)
        {
            // 计算当周的开始日期
            var weekStartDate = GetWeekStartDate(date).Date;
            var weekEndDate = weekStartDate.AddDays(6);
            // 查询当周所有折扣不为零的周菜单记录
            var weekMenus = await _weekMenuRepository.FindByConditionAsync(wm =>
                wm.Week >= weekStartDate.Date && wm.Week<=weekEndDate.Date && wm.DisPrice > 0);

            var dishIds = weekMenus.Select(wm => wm.DishId).Distinct().ToList();
            var dishes = new List<DishDto>();

            foreach (var dishId in dishIds)
            {
                // 获取菜品信息
                var dish = await _dishRepository.GetByIdAsync(dishId);
                if (dish == null) continue;

                // 获取菜品的原价和折扣价格
                var weekMenu = weekMenus.First(wm => wm.DishId == dishId);
                var originalPrice = dish.Price;
                var currentPrice = weekMenu.DisPrice;

                // 查询菜品类别
                var category = await _cateRepository.GetByIdAsync(dish.CateId);
                var categoryName = category?.CateName;

                // 添加到返回结果中
                dishes.Add(new DishDto
                {
                    Id = dishId,
                    Name = dish.DishName,
                    OriginalPrice = originalPrice,
                    CurrentPrice = currentPrice,
                    Category = categoryName
                });
            }

            return new AllDiscountResponseDto
            {
                Dishes = dishes
            };
        }
/*
        public async Task<WmResponseDto> AddWM(WmRequestDto request)
        {
            var dishId = request.DishId;
            var day = request.Day;
            var existedDish = await _dishRepository.GetByIdAsync(dishId);
            if (existedDish == null)
            {
                return new WmResponseDto
                {
                    Success = false,
                    Message = "not found"
                };
            }

            var weekDate = GetWeekStartDate(request.Date);

            var existedWM = await _weekMenuRepository.FindByCompositeKeyAsync<Weekmenu>(dishId, weekDate);
            if (existedWM != null)
            {
                return new WmResponseDto
                {
                    Success = false,
                    Message = "already existed",

                };
            }
            var newWM = new Weekmenu
            {
                DishId = dishId,
                Week = weekDate,
                Stock = 50,//先写作默认
                Sales = 0,
                DisPrice = 0,
                Day = day,
            };
            await _weekMenuRepository.AddAsync(newWM);
            var category = await _cateRepository.GetByIdAsync(existedDish.CateId);
            return new WmResponseDto
            {
                Success = true,
                Message = "success",
                Dish = new DishInfo
                {
                    Category = category.CateName,
                    DishName = existedDish.DishName
                }
            };


        }

        public async Task<AllWeekMenuResponseDto> GetWeekMenuByDateAsync(DateTime date)
        {
            // 计算当周的开始日期
            var weekStartDate = GetWeekStartDate(date);

            // 查询数据库中符合条件的所有条目
            var weekMenus = await _weekMenuRepository.FindByConditionAsync(wm =>
                wm.Week == weekStartDate);
            if (!weekMenus.Any())
            {
                return null;
            }
            var response = new AllWeekMenuResponseDto
            {
                Mon = new List<Mon>(),
                Tue = new List<Tue>(),
                Wed = new List<Wed>(),
                Thu = new List<Thu>(),
                Fri = new List<Fri>(),
                Sat = new List<Sat>(),
                Sun = new List<Sun>()
            };

            foreach (var weekMenu in weekMenus)
            {
                // 根据 dishId 从 Dish 表中获取 Dish 实体
                var dish = await _dishRepository.GetByIdAsync(weekMenu.DishId);
                if (dish == null) continue; // 跳过未找到的 dish

                // 根据 cateId 从 Category 表中获取 Category 实体
                var category = await _cateRepository.GetByIdAsync(dish.CateId);
                var categoryName = category?.CateName; // 获取 categoryName

                var menuDto = new
                {
                    Category = categoryName,
                    Id = weekMenu.DishId,
                    Name = dish.DishName
                };

                // 根据 weekMenu.Day 的值将菜单项分类到对应的列表中
                switch (weekMenu.Day)
                {
                    case "Mon":
                        response.Mon.Add(new Mon { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Tue":
                        response.Tue.Add(new Tue { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Wed":
                        response.Wed.Add(new Wed { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Thu":
                        response.Thu.Add(new Thu { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Fri":
                        response.Fri.Add(new Fri { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Sat":
                        response.Sat.Add(new Sat { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                    case "Sun":
                        response.Sun.Add(new Sun { Category = menuDto.Category, Id = menuDto.Id, Name = menuDto.Name });
                        break;
                }
            }

            return response;
        }


        public async Task<Weekmenu> FindByCompositeAsync(string dishId, DateTime weekDate, string day)
        {
            // 使用 FindByConditionAsync 查找符合条件的实体
            var entities = await _weekMenuRepository.FindByConditionAsync(wm =>
                wm.DishId.Equals(dishId) &&
                wm.Week.Equals(weekDate) &&
                wm.Day.Equals(day)
            );

            // 返回找到的第一个实体（如果有的话）
            return entities.FirstOrDefault();
        }

        public async Task<bool> DeleteByCompositeAsync(string dishId, DateTime weekDate, string day)
        {
            return await _weekMenuRepository.DeleteByConditionAsync(wm =>
                wm.DishId.Equals(dishId) &&
                wm.Week.Equals(weekDate) &&
                wm.Day.Equals(day)
            );
        }

        public async Task<WmResponseDto> RemoveWM(WmRequestDto request)
        {
            var dishId = request.DishId;
            var day = request.Day;
            var existedDish = await _dishRepository.GetByIdAsync(dishId);
            if (existedDish == null)
            {
                return new WmResponseDto
                {
                    Success = false,
                    Message = "Dish not found"
                };
            }

            var weekDate = GetWeekStartDate(request.Date);

            var existedWM = await FindByCompositeAsync(dishId, weekDate, day);
            if (existedWM == null)
            {
                return new WmResponseDto
                {
                    Success = false,
                    Message = "Dish does not exist in this week or in this day",
                };
            }

            var deleteSuccess = await DeleteByCompositeAsync(dishId, weekDate, day);
            if (!deleteSuccess)
            {
                return new WmResponseDto
                {
                    Success = false,
                    Message = "Failed to delete"
                };
            }

            return new WmResponseDto
            {
                Success = true,
                Message = "Successfully deleted"
            };
        }

        public async Task<DiscountResponseDto> UploadDiscount(DiscountRequestDto dto)
        {
            var weekDate = GetWeekStartDate(dto.Date);
            var discount = dto.Discount;
            var dishId = dto.DishId;

            var existedDish = await _weekMenuRepository.FindByCompositeKeyAsync<Weekmenu>(dishId, weekDate);
            if (existedDish == null)
            {
                return new DiscountResponseDto
                {
                    Success = false,
                    Message = "dish not found in this week"
                };
            }
            var dishPrice = (await _dishRepository.GetByIdAsync(dishId)).Price;
            decimal disPrice = dishPrice * dto.Discount;
            existedDish.DisPrice = disPrice;
            await _weekMenuRepository.UpdateAsync(existedDish);
            return new DiscountResponseDto
            {
                Success = true,
                Message = "success",
                DishId = dishId,
                UpdatedPrice = disPrice
            };
        }

        public async Task<BatchResponseDto> BatchDiscount(BatchRequestDto dto)
        {
            var weekDate = GetWeekStartDate(dto.Date);
            var discount = dto.Discount;
            var dishIds = dto.DishIds;

            var response = new BatchResponseDto
            {
                DiscountDishes = new List<DiscountDish>(),
                Success = true,
                Message = "Batch discount applied successfully."
            };

            foreach (var dishId in dishIds)
            {
                // 查找指定 dishId 的周菜单记录
                var existedDish = await _weekMenuRepository.FindByCompositeKeyAsync<Weekmenu>(dishId, weekDate);
                if (existedDish == null)
                {
                    response.Success = false;
                    response.Message = $"Dish with ID {dishId} not found in this week.";
                    continue;
                }

                // 获取菜品信息
                var dish = await _dishRepository.GetByIdAsync(dishId);
                if (dish == null)
                {
                    response.Success = false;
                    response.Message = $"Dish with ID {dishId} not found.";
                    continue;
                }

                // 计算折扣后的价格
                var discountPrice = dish.Price * discount;
                existedDish.DisPrice = discountPrice;

                // 更新周菜单记录
                await _weekMenuRepository.UpdateAsync(existedDish);

                // 添加到返回的结果中
                response.DiscountDishes.Add(new DiscountDish
                {
                    DishId = dishId,
                    DishName = dish.DishName,
                    DiscountPrice = discountPrice
                });
            }

            return response;
        }
        public async Task<AllDiscountResponseDto> GetAllDiscount(DateTime date)
        {
            // 计算当周的开始日期
            var weekStartDate = GetWeekStartDate(date);

            // 查询当周所有折扣不为零的周菜单记录
            var weekMenus = await _weekMenuRepository.FindByConditionAsync(wm =>
                wm.Week == weekStartDate && wm.DisPrice > 0);

            var dishIds = weekMenus.Select(wm => wm.DishId).Distinct().ToList();
            var dishes = new List<DishDto>();

            foreach (var dishId in dishIds)
            {
                // 获取菜品信息
                var dish = await _dishRepository.GetByIdAsync(dishId);
                if (dish == null) continue;

                // 获取菜品的原价和折扣价格
                var weekMenu = weekMenus.First(wm => wm.DishId == dishId);
                var originalPrice = dish.Price;
                var currentPrice = weekMenu.DisPrice;

                // 查询菜品类别
                var category = await _cateRepository.GetByIdAsync(dish.CateId);
                var categoryName = category?.CateName;

                // 添加到返回结果中
                dishes.Add(new DishDto
                {
                    Id = dishId,
                    Name = dish.DishName,
                    OriginalPrice = originalPrice,
                    CurrentPrice = currentPrice,
                    Category = categoryName
                });
            }

            return new AllDiscountResponseDto
            {
                Dishes = dishes
            };
        }*/

    }
}
