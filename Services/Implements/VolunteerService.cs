using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Dtos.Register;
using Elderly_Canteen.Data.Dtos.Volunteer;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Elderly_Canteen.Data.Dtos.Account;
using Elderly_Canteen.Data.Dtos.Donate;

namespace Elderly_Canteen.Services.Implements
{
    public class VolunteerService : IVolunteerService
    {
        private readonly IGenericRepository<VolApplication> _applicationRepository;
        private readonly IGenericRepository<VolReview> _reviewRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<Volunteer> _volunteerRepository;
        public VolunteerService(IGenericRepository<Account> accountRepository, IGenericRepository<VolApplication> applicationRepository, IGenericRepository<Volunteer> volunteerRepository, IGenericRepository<VolReview> reviewRepository)
        {
            _applicationRepository = applicationRepository;
            _accountRepository = accountRepository;
            _volunteerRepository = volunteerRepository;
            _reviewRepository = reviewRepository;
        }

        //志愿者申请
        public async Task ApplyAsync(VolunteerApplicationDto application, string accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                throw new InvalidOperationException("用户不存在");
            }
            if (account.Idcard == null)
            {
                throw new InvalidOperationException("用户未实名认证");
            }
            var vol = await _volunteerRepository.GetByIdAsync(accountId);
            if (vol != null)
            {
                throw new InvalidOperationException("用户已是志愿者");
            }

            var applyList = await _applicationRepository.FindByConditionAsync(e => e.AccountId == accountId);
            if (applyList != null && applyList.Any())
            {
                foreach (var apply in applyList)
                {
                    var rev = await _reviewRepository.FindByConditionAsync(e => e.ApplicationId == apply.ApplicationId);
                    if (!rev.Any()) // 如果没有找到对应的审核记录
                    {
                        throw new InvalidOperationException("用户上次申请还未审核");
                    }
                }
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

        //获得所有未审核申请（就是申请表）逻辑
        public async Task<VolunteerReviewListDto> GetAllApplyAsync()
        {
            var applications = await _applicationRepository.GetAllAsync();

            if (applications == null || !applications.Any())
            {
                return new VolunteerReviewListDto
                {
                    success = false,
                    msg = "暂无申请表",
                    responce = null
                };
            }

            var responseList = new List<VolunteerReviewListDto.ResponceData>();

            foreach (var application in applications)
            {
                // 查找审核表中是否有对应的审核记录
                var reviewExists = await _reviewRepository.FindByConditionAsync(r => r.ApplicationId == application.ApplicationId);

                if (!reviewExists.Any()) // 如果审核表中没有对应的记录
                {
                    var response = new VolunteerReviewListDto.ResponceData
                    {
                        applicationId = application.ApplicationId,
                        accountId = application.AccountId,
                        applicationDate = application.ApplicationDate.ToString("yyyy-MM-dd")
                    };

                    responseList.Add(response);
                }
            }

            if (!responseList.Any())
            {
                return new VolunteerReviewListDto
                {
                    success = false,
                    msg = "所有申请均已审核",
                    responce = null
                };
            }

            return new VolunteerReviewListDto
            {
                success = true,
                msg = "成功",
                responce = responseList
            };
        }


        //获得某个申请者的详细信息逻辑
        public async Task<VolunteerReviewInfoDto> GetApplyInfoAsync(string applyId)
        {
            var application = await _applicationRepository.GetByIdAsync(applyId);
            if (application == null)
            {
                return new VolunteerReviewInfoDto
                {
                    success = false,
                    msg = "该用户暂无申请",
                    responce = null
                };
            }

            var accountId = application.AccountId;
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new VolunteerReviewInfoDto
                {
                    success = false,
                    msg = "用户不存在",
                    responce = null
                };
            }
            if (account.Idcard == null)
            {
                return new VolunteerReviewInfoDto
                {
                    success = false,
                    msg = "用户未实名认证",
                    responce = null
                };
            }
            var vol = await _volunteerRepository.GetByIdAsync(accountId);
            if (vol != null)
            {
                return new VolunteerReviewInfoDto
                {
                    success = false,
                    msg = "用户已是志愿者",
                    responce = null
                };
            }
          
            var res = new VolunteerReviewInfoDto.ResponceData
            {
                name = account.Name,
                gender = account.Gender,
                birthDate = account.Birthdate.HasValue
                            ? account.Birthdate.Value.ToString("yyyy-MM-dd")
                            : string.Empty,

                phoneNum = account.Phonenum,
                idCard = account.Idcard,
                selfStatement = application.SelfStatement
            };

            var applicationDto = new VolunteerReviewInfoDto
            {
                success = true,
                msg = "获取用户申请成功",
                responce = res
            };

            return applicationDto;
        }

        // 审核逻辑
        public async Task ReviewApplicationAsync(VolunteerReviewApplicationDto application, string id , string accountId)
        {
            var apply = await _applicationRepository.GetByIdAsync(id);
            if (apply == null)
            {
                throw new InvalidOperationException($"ID为 {id} 的申请表不存在");
            }

            var review = await _reviewRepository.GetByIdAsync(id);
            if (review != null)
            {
                throw new InvalidOperationException($"ID为 {id} 的申请表已审核");
            }

            //加入到审核表中
            var recv = new VolReview
            {
                ApplicationId = apply.ApplicationId, 
                AdministratorId=accountId,
                Reason = application.reason,
                Status = application.status,
                ReviewDate = DateTime.Now,
            };
            await _reviewRepository.AddAsync(recv);

            //是否加入志愿者表
            if(application.status=="通过")
            {
                var volunteer = new Volunteer
                {
                    AccountId = apply.AccountId, 
                    Available = "1",
                    Score = null
                };
                await _volunteerRepository.AddAsync(volunteer);
            }
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
