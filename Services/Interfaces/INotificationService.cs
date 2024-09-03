namespace Elderly_Canteen.Services.Interfaces
{
    public interface INotificationService
    {
        void NotifyLowStock(string dishId);
        void NotifyExpiringSoon(string ingredientId, DateTime expirationDate);
    }

}
