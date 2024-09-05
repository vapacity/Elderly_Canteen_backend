using Elderly_Canteen.Data.Dtos.Finance;
using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IFinanceService
    {
        //获取财务列表
        Task<FinanceResponseDto> GetFilteredFinanceInfoAsync(string financeType = null, string inOrOut = null, string financeDate = null, string financeId = null, string accountId = null, string status = null);
        //更新审核状态
        Task<FinanceResponseDto> UpdateFinanceStatusAsync(string financeId, string status, string administratorId);
        Task<object> DeductBalanceAsync(string accountId, decimal amount);
        //Task<string> ProcessSubsidyAsync(string accountId, decimal subsidyAmount);
        //获取财务净收入、总收入、总支出
        Task<FinanceTotalsResponse> GetTotalFinanceInfo();

    }
}
