namespace Elderly_Canteen.Services.Interfaces
{
    public interface IFinanceService
    {
        Task<object> DeductBalanceAsync(string accountId, decimal amount);
        //Task<string> ProcessSubsidyAsync(string accountId, decimal subsidyAmount);
    }
}
