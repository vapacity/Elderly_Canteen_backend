using Elderly_Canteen.Data.Dtos.Finance;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Elderly_Canteen.Data.Dtos.Finance;

public class FinanceService : IFinanceService

namespace Elderly_Canteen.Services.Implements
{
    private readonly IGenericRepository<Account> _accountRepository;
    private readonly IGenericRepository<Senior> _seniorRepository;
    private readonly IGenericRepository<Finance> _financeRepository;

    public FinanceService(IGenericRepository<Account> accountRepository, IGenericRepository<Finance> financeRepository, IGenericRepository<Senior> seniorRepository)
    {
        _accountRepository = accountRepository;
        _financeRepository = financeRepository;
        _seniorRepository = seniorRepository;
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
    // 扣除用户余额的逻辑
    public async Task<object> DeductBalanceAsync(string accountId, decimal totalPrice)
    {
        // 1. 获取用户信息
        var account = await _accountRepository.GetByIdAsync(accountId);
        var identity = account.Identity;
        decimal money = account.Money;
        decimal bonus = 0m;
        if (identity == "senior")
        {
            bonus = totalPrice * 0.2m;
            var senior = await _seniorRepository.GetByIdAsync(accountId);
            if (bonus < senior.Subsidy)
                bonus = 0m;
            totalPrice -= bonus;
        }
        // 2. 检查余额是否足够
        if (money < totalPrice)
        {
            return new
            {
                Success = false,
                Msg = "余额不足"
            };
        }

        // 3. 扣除余额，并更新用户信息
        account.Money -= totalPrice;
        await _accountRepository.UpdateAsync(account);

        // 4. 记录财务信息到Finance表中
        string financeId = await GenerateFinanceIdAsync();
        var newFinance = new Finance
        {
            FinanceId = financeId,
            FinanceType = "点单",
            FinanceDate = DateTime.Now,
            Price = totalPrice,
            InOrOut = "0",
            AccountId = accountId,
            Status = "待审核"
        };
        await _financeRepository.AddAsync(newFinance);

        return new
        {
            Success = true,
            Msg = "success",
            FinanceId = financeId,
            bonus = bonus
        };

    }
    // 处理老人补贴的逻辑
    async Task<string> ProcessSubsidyAsync(string accountId, decimal subsidyAmount)
    {
        // 1. 根据老人政策，计算应扣除的补贴金额
        // 2. 从老人的补贴账户中扣除补贴
        // 3. 记录财务信息，更新余额和补贴
        return null;
    }

    async Task<string> GenerateFinanceIdAsync()
    {
        const string prefix = "200";

        var maxFinanceId = await _financeRepository.GetAll()
            .Where(f => f.FinanceId.StartsWith(prefix))
            .OrderByDescending(f => f.FinanceId)
            .Select(f => f.FinanceId)
            .FirstOrDefaultAsync();

        if (maxFinanceId == null)
        {
            return prefix + "00001";
        }

        // 提取数字部分并加1
        var numericalPart = int.Parse(maxFinanceId.Substring(prefix.Length));
        var newFinanceId = numericalPart + 1;

        // 使用前导零格式化新的Finance ID
        return prefix + newFinanceId.ToString("D5");
    }

    //获取财务净收入、总收入、总支出
    public async Task<FinanceTotalsResponse> GetTotalFinanceInfo()
    {
        var finances = await _financeRepository.GetAllAsync();
        if (finances == null || !finances.Any())
        {
            return new FinanceTotalsResponse
            {
                success = false,
                msg = "未找到任何财务信息"
            };
        }

        var approvedFinances = finances.Where(f => f.Status == "已通过");
        if (approvedFinances == null || !approvedFinances.Any())
        {
            return new FinanceTotalsResponse
            {
                success = false,
                msg = "未找到任何审核状态为已通过的财务信息"
            };
        }
        decimal totalIncome = approvedFinances
            .Where(f => f.InOrOut == "0" || f.InOrOut == "收入")
            .Sum(f => f.Price);
        decimal totalExpense = approvedFinances
            .Where(f => f.InOrOut == "1" || f.InOrOut == "支出")
            .Sum(f => f.Price);
        decimal netIncome = totalIncome - totalExpense;

        return new FinanceTotalsResponse
        {
            success = true,
            msg = "财务信息统计完成！",
            response = new FinanceData
            {
                NetIn = netIncome,
                TotalIn = totalIncome,
                TotalOut = totalExpense
            }
        };
    }
}

