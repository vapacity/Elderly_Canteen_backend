using Elderly_Canteen.Data.Dtos.WeekMenu;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IWeekMenuService
    {
        Task<WmResponseDto> AddWM(WmRequestDto request);
        Task<WmResponseDto> RemoveWM(WmRequestDto request);
        Task<AllWeekMenuResponseDto> GetWeekMenuByDateAsync(DateTime date);
        Task<DiscountResponseDto> UploadDiscount(DiscountRequestDto dto);
        Task<BatchResponseDto> BatchDiscount(BatchRequestDto dto);
        Task<AllDiscountResponseDto> GetAllDiscount(DateTime date);
    }
}
