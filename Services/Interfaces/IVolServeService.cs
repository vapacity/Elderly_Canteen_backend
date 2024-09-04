using Elderly_Canteen.Data.Dtos.VolServe;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IVolServeService
    {
        Task<AccessOrderResponseDto> GetAccessOrder();
        Task<NormalResponseDto> AcceptOrderAsync(string orderId, string accountId);
        Task<NormalResponseDto> ConfirmDeliveredAsync(string orderId,string accountId);
        Task<AccessOrderResponseDto> GetAcceptedOrder(string accountId);
        Task<AccessOrderResponseDto> GetFinishedOrder(string accountId);
    }
}
