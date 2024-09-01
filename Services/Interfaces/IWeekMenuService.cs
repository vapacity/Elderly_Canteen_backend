using Elderly_Canteen.Data.Dtos.WeekMenu;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IWeekMenuService
    {
        Task<WmResponseDto> AddWM(WmRequestDto request);
        Task<WmResponseDto> RemoveWM(WmRequestDto request);
        Task<WmResponseDto> GetWM(WmRequestDto request);
    }
}
