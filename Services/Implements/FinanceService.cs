using Elderly_Canteen.Data.Dtos.Finance;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class FinanceService: IFinanceService
{
    private readonly IGenericRepository<Finance> _financeRepository;
    public FinanceService(IGenericRepository<Finance> financeRepository)
    {
        _financeRepository = financeRepository;
    }
    //获取所有财务信息
    public async Task<FinanceResponseDto> GetFilteredFinanceInfoAsync(string financeType = null, string inOrOut = null, string financeDate = null, string financeId = null, string accountId = null)
    {
        var query = _financeRepository.GetAll();

        if (!string.IsNullOrEmpty(financeType))
        {
            query = query.Where(f => f.FinanceType == financeType);
        }
        if (!string.IsNullOrEmpty(inOrOut))
        {
            query = query.Where(f => f.InOrOut == inOrOut);
        }
        if (!string.IsNullOrEmpty(financeDate))
        {
            DateTime date;
            if (DateTime.TryParse(financeDate, out date))
            {
                query = query.Where(f => f.FinanceDate == date.Date);
            }
        }
        if (!string.IsNullOrEmpty(financeId))
        {
            query = query.Where(f => f.FinanceId == financeId);
        }
        if (!string.IsNullOrEmpty(accountId))
        {
            query = query.Where(f => f.AccountId == accountId);
        }

        var finances = await query.ToListAsync();

        if (!finances.Any())
        {
            return new FinanceResponseDto
            {
                success = false,
                msg = "未找到符合条件的财务信息"
            };
        }

        var financeList = finances.Select(finance => new FinanceResponseData
        {
            FinanceId = finance.FinanceId,
            FinanceType = finance.FinanceType,
            FinanceDate = finance.FinanceDate,
            Price = finance.Price,
            InOrOut = finance.InOrOut,
            AccountId = finance.AccountId,
            AdministratorId = finance.AdministratorId,
            Proof = finance.Proof,
            Status = finance.Status
        }).ToList();

        return new FinanceResponseDto
        {
            success = true,
            msg = "财务信息检索成功",
            response = financeList
        };
    }

    //审核财务信息
    public async Task<FinanceResponseDto> UpdateFinanceStatusAsync(string financeId, string status, string administratorId)
    {
        var finance = await _financeRepository.GetByIdAsync(financeId);
        if (finance == null)
        {
            return new FinanceResponseDto
            {
                success = false,
                msg = "找不到指定的财务记录"
            };
        }

        finance.Status = status;
        finance.AdministratorId = administratorId;

        await _financeRepository.UpdateAsync(finance);

        return new FinanceResponseDto
        {
            success = true,
            msg = $"财务记录状态已更新为 {status}"
        };
    }
}


