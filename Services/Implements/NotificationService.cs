using Elderly_Canteen.Services.Interfaces;

namespace Elderly_Canteen.Services.Implements
{
    public class NotificationService : INotificationService
    {
        public void NotifyLowStock(string dishId)
        {
            // 1. 发送库存低的通知（如发送邮件或系统消息）
            // 2. 记录低库存警告日志
            return;
        }

        public void NotifyExpiringSoon(string ingredientId, DateTime expirationDate)
        {
            string message = $"食材 {ingredientId} 将在 {expirationDate.ToShortDateString()} 过期，请及时处理。";
            // 发送通知逻辑，如通过WebSocket、邮件等
            //_webSocketHandler.SendMessageAsync(message); // 示例：使用WebSocket发送通知
            return;
        }

    }

}
