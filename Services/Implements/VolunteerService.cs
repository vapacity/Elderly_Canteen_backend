using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Dtos.Register;
using Elderly_Canteen.Data.Dtos.Volunteer;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Elderly_Canteen.Services.Implements
{
    public class VolunteerService : IVolunteerService
    {
        private readonly IGenericRepository<VolApplication> _applicationRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        public VolunteerService(IGenericRepository<Account> accountRepository, IGenericRepository<VolApplication> applicationRepository)
        {
            _applicationRepository = applicationRepository;
            _accountRepository = accountRepository;
        }

        public async Task ApplyAsync(VolunteerApplicationDto application, string accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                throw new InvalidOperationException("用户不存在");
            }
            var newApplication = new VolApplication
            {
                ApplicationId = await GenerateApplicationIdAsync(),
                AccountId = accountId,
                SelfStatement = application.selfStatement,
                ApplicationDate = DateTime.Now
            };

            await _applicationRepository.AddAsync(newApplication);
        }


        // 辅助函数：根据当前申请表最大ID生成新的ID
        private async Task<string> GenerateApplicationIdAsync()
        {
            // 获取数据库中当前的最大ID
            var maxApplicationId = await _applicationRepository.GetAll()
                .OrderByDescending(e => e.ApplicationId)
                .Select(e => e.ApplicationId)
                .FirstOrDefaultAsync();

            // 如果没有记录，返回第一个ID
            if (maxApplicationId == null)
            {
                return "VA00001";
            }

            // 提取数字部分，并加1
            int numericPart = int.Parse(maxApplicationId.Substring(2)); // 改为 Substring(2)
            int newNumericPart = numericPart + 1;

            // 新的ID为VA + (最大数字 + 1), 且ID是5位数字，不足时左侧填充0
            return $"VA{newNumericPart.ToString("D5")}";
        }

    }
}
