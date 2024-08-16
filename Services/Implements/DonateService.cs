using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Elderly_Canteen.Data.Dtos.Login;
using Microsoft.EntityFrameworkCore;
using Elderly_Canteen.Data.Dtos.Register;
using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Dtos.Account;
using Elderly_Canteen.Data.Dtos.AuthenticationDto;
using Elderly_Canteen.Data.Dtos.Donate;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Elderly_Canteen.Services.Implements
{
    public class DonateService : IDonateService
    {
        private readonly IDonateRepository<Donation> _donateRepository;
        private readonly IFinanceRepository<Finance> _financeRepository;
        private readonly IConfiguration _configuration;

        public DonateService(IDonateRepository<Donation> donateRepository, IFinanceRepository<Finance> financeRepository, IConfiguration configuration)
        {
            _donateRepository = donateRepository ?? throw new ArgumentNullException(nameof(donateRepository));
            _financeRepository = financeRepository ?? throw new ArgumentNullException(nameof(financeRepository));
            _configuration = configuration;
        }

        //获得捐赠列表逻辑
        public async Task<DonationListDto> GetDonateListAsync()
        {
            var donations = await _donateRepository.GetListAsync();

            var responseDataList = new List<DonationListDto.ResponceData>();

            foreach (var donation in donations)
            {
                // 创建 ResponceData 实例并设置属性
                var responceData = new DonationListDto.ResponceData
                {
                    origin = donation.Origin,
                    price = (double)donation.Finance.Price,  // 从关联的Finance表获取金额
                    financeDate = donation.Finance.FinanceDate.ToString("yyyy-MM-dd")  // 格式化日期为 "yyyy-MM-dd"
                };

                responseDataList.Add(responceData);
            }

            var donationDto = new DonationListDto
            {
                success = true,
                msg = "成功",
                responce = responseDataList // 将responseDataList赋值给responce
            };

            return donationDto;
        }

        // 增加捐赠信息
        public async Task<DonateResponseDto> SubmitDonationAsync(DonateRequestDto request)
        {
            try
            {
                // 生成 FinanceId
                var financeId = await GenerateFinanceIdAsync();

                // 插入财务信息
                var finance = new Finance
                {
                    FinanceId = financeId,
                    FinanceType = "捐赠",
                    FinanceDate = DateTime.Now,
                    Price = (decimal)request.price,
                    InOrOut = "0",
                    AccountId = request.accountId,
                    AdministratorId = null, // 可空
                    Proof = null,           // 可空
                    Status = "待审核"
                };

                await _financeRepository.AddAsync(finance);

                // 生成 DonationId
                var donationId = await GenerateDonationIdAsync();
                
                var donation = new Donation
                {
                    DonationId = donationId,
                    FinanceId = financeId,
                    Origin = request.origin,
                    AccountId = request.accountId
                };

                await _donateRepository.AddAsync(donation);

                return new DonateResponseDto
                {
                    success = true,
                    msg = "Donation submitted successfully."
                };
            }
            catch (Exception ex)
            {
                return new DonateResponseDto
                {
                    success = false,
                    msg = $"Failed to submit donation: {ex.Message}"
                };
            }
        }

        //生成捐赠ID
        private async Task<string> GenerateDonationIdAsync()
        {
            const string prefix = "211";

            var maxDonateId = await _donateRepository.GetAll()
                .Where(a => a.DonationId.StartsWith(prefix))
                .OrderByDescending(a => a.DonationId)
                .Select(a => a.DonationId)
                .FirstOrDefaultAsync();

            if (maxDonateId == null)
            {
                return prefix + "00001";
            }

            // Extract the numerical part and increment it by 1
            var numericalPart = int.Parse(maxDonateId.Substring(prefix.Length));
            var newDonateId = numericalPart + 1;

            // Format the new Donate ID with leading zeros
            return prefix + newDonateId.ToString("D5");
        }
        //生成财务ID
        private async Task<string> GenerateFinanceIdAsync()
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

    }
}
