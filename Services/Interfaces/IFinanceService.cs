using Elderly_Canteen.Data.Dtos.Finance;
using Elderly_Canteen.Data.Entities;

namespace Elderly_Canteen.Services.Interfaces
{
    public interface IFinanceService
    {
        //获取财务列表
        Task<FinanceResponseDto> GetFilteredFinanceInfoAsync(string? financeType = null, string inOrOut = null, string financeDate = null, string financeId = null, string accountId = null);
        //更新审核状态
        Task<FinanceResponseDto> UpdateFinanceStatusAsync(string financeId, string status, string administratorId);

    }
}
