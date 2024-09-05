using Elderly_Canteen.Data.Dtos.Order;
using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IOrderService
    {
        Task<MenuResponseDto> GetMenuToday();

        // 创建订单逻辑
        Task<OrderInfoDto> CreateOrderAsync(string cartId, string accountId, string? newAddress,bool deliver_or_dining,string financeId,List<CartItem> cartItems);

        Task<decimal> CalculateTotalPrice(List<CartItem> cartItems);


        // 返回历史所有订单
        Task<GetOrderResponseDto> GetHistoryOrderInfoAsync(string accountId);

        Task<NormalResponseDto> ConfirmOrderAsync(string orderId,string accountId);

        Task<GetOdMsgResponseDto> GetODMsg(string orderId);

        Task<dynamic> SubmitDiningReviewAsync(ReviewSubmissionDto review);

        Task<ReviewResponseDto> GetReviewByOrderId(string orderId);
        Task<dynamic> SubmitDeliveringReviewAsync(ReviewSubmissionDto review);
        Task<ReviewResponseDto> GetDeliveringReviewByOrderId(string orderId);
        Task<OrderInfoDto> GetOrderInfoByIdAsync(string orderId);


    }
}
