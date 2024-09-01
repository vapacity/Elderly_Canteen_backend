using Elderly_Canteen.Data.Dtos.WeekMenu;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;

namespace Elderly_Canteen.Services.Implements
{
    public class WeekMenuService:IWeekMenuService
    {
        private readonly IGenericRepository<Weekmenu> _weekMenuRepository;
        private readonly IGenericRepository<Dish> _dishRepository;
        private readonly IGenericRepository<Category> _cateRepository;
        public WeekMenuService(IGenericRepository<Weekmenu> weekMenuRepository,
                                IGenericRepository<Dish> dishRepository,
                                IGenericRepository<Category> cateRepository)
        {
            _weekMenuRepository = weekMenuRepository;
            _dishRepository = dishRepository;
            _cateRepository = cateRepository;
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

        public async Task<WmResponseDto> AddWM(WmRequestDto request)
        {
            var dishId = request.DishId;
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
            var newWM = new Weekmenu
            {
                DishId = dishId,
                Week = weekDate,
                Stock = 50,//先写作默认
                Sales = 0,
                DisPrice = 0,
            };
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
        
        public async Task<WmResponseDto> GetWM(WmRequestDto request)
        {
            return null;
        }
        public async Task<WmResponseDto> RemoveWM(WmRequestDto request)
        {
            return null;
        }
    }
}
