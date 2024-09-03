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
using static System.Net.Mime.MediaTypeNames;

namespace Elderly_Canteen.Services.Implements
{
    public class VolunteerService : IVolunteerService
    {
        private readonly IGenericRepository<VolApplication> _applicationRepository;
        private readonly IGenericRepository<VolReview> _reviewRepository;
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<Volunteer> _volunteerRepository;
        private readonly IGenericRepository<DeliverV> _delivervRepository;
        private readonly IGenericRepository<Senior> _seniorRepository;
        public VolunteerService(IGenericRepository<Account> accountRepository, 
            IGenericRepository<VolApplication> applicationRepository, 
            IGenericRepository<Volunteer> volunteerRepository, 
            IGenericRepository<VolReview> reviewRepository, 
            IGenericRepository<DeliverV> delivervRepository,
            IGenericRepository<Senior> seniorRepository)
        {
            _applicationRepository = applicationRepository;
            _accountRepository = accountRepository;
            _volunteerRepository = volunteerRepository;
            _reviewRepository = reviewRepository;
            _delivervRepository = delivervRepository;
            _seniorRepository = seniorRepository;
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
            var senior = await _seniorRepository.GetByIdAsync(accountId);
            if (senior != null)
            {
                throw new InvalidOperationException("60岁以上老人不能申请志愿者");
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

        //获得所有未审核申请逻辑
        public async Task<VolunteerReviewListDto> GetAllApplyAsync()
        {
            var applications = await _applicationRepository.GetAllAsync();

            if (applications == null || !applications.Any())
            {
                return new VolunteerReviewListDto
                {
                    success = false,
                    msg = "暂无申请表",
                    response = null
                };
            }

            var responseList = new List<VolunteerReviewListDto.ResponseData>();

            foreach (var application in applications)
            {
                // 查找审核表中是否有对应的审核记录
                var reviewExists = await _reviewRepository.FindByConditionAsync(r => r.ApplicationId == application.ApplicationId);

                if (!reviewExists.Any()) // 如果审核表中没有对应的记录
                {
                    var response = new VolunteerReviewListDto.ResponseData
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
                    response = null
                };
            }

            return new VolunteerReviewListDto
            {
                success = true,
                msg = "成功",
                response = responseList
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
                    response = null
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
                    response = null
                };
            }
            if (account.Idcard == null)
            {
                return new VolunteerReviewInfoDto
                {
                    success = false,
                    msg = "用户未实名认证",
                    response = null
                };
            }
            var vol = await _volunteerRepository.GetByIdAsync(accountId);
            if (vol != null)
            {
                return new VolunteerReviewInfoDto
                {
                    success = false,
                    msg = "用户已是志愿者",
                    response = null
                };
            }
          
            var res = new VolunteerReviewInfoDto.ResponseData
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
                response = res
            };

            return applicationDto;
        }

        // 审核逻辑
        public async Task ReviewApplicationAsync(VolunteerReviewApplicationDto application, string id , string accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null) {
                throw new InvalidOperationException($"ID为 {id} 的用户不存在");
            }

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

                var accountToChange= await _accountRepository.GetByIdAsync(apply.AccountId);
                accountToChange.Identity = "volunteer";
                await _accountRepository.UpdateAsync(account);
            }
        }

        public async Task<VolunteerResponseDto> GetVolInfoAsync(string accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new VolunteerResponseDto
                {
                    success = false,
                    msg = "用户不存在",
                    response = null
                };
            }

            var vol = await _volunteerRepository.GetByIdAsync(accountId);
            if (vol == null)
            {
                return new VolunteerResponseDto
                {
                    success = false,
                    msg = "用户不是志愿者",
                    response = null
                };
            }

            var num = await _delivervRepository.CountAsync(e => e.VolunteerId == accountId);
            
            var applyList = await _applicationRepository.FindByConditionAsync(e => e.AccountId == accountId);
            if (applyList == null || !applyList.Any())
            {
                return new VolunteerResponseDto
                {
                    success = false,
                    msg = "用户暂无申请表",
                    response = null
                };
            }
            foreach (var application in applyList)
            {
                // 查找审核表中是否有对应的审核记录
                var review = await _reviewRepository.GetByIdAsync(application.ApplicationId);

                if (review.Status=="通过") // 如果审核表中没有对应的记录
                {
                    return new VolunteerResponseDto
                    {
                        success = true,
                        msg = "获取成功",
                        response = new VolunteerResponseDto.ResponseData
                        {
                            accountId = account.Accountid,
                            name = account.Name,
                            deliverCount = num,
                            time = review.ReviewDate.ToString("yyyy-MM-dd")
                        }
                    };
                }
            }
            return new VolunteerResponseDto
            {
                success = false,
                msg = "用户志愿者申请未被通过",
                response = null
            };

        }

        //获取所有志愿者
        public async Task<VolunteerListDto> GetAllVolunteerAsync()
        {
            var volunteers = await _volunteerRepository.GetAllAsync();

            if (volunteers == null || !volunteers.Any())
            {
                return new VolunteerListDto
                {
                    success = false,
                    msg = "暂无志愿者",
                    response = null
                };
            }

            var responseList = new List<VolunteerListDto.ResponseData>();

            foreach (var volunteer in volunteers)
            {
                var account = await _accountRepository.GetByIdAsync(volunteer.AccountId);

                var iscore = 0.0;
                if (volunteer.Score != null)
                {
                    iscore = (double)volunteer.Score;
                }

                var response = new VolunteerListDto.ResponseData
                {
                    name = account.Name,
                    accountId = volunteer.AccountId,
                    idCard = account.Idcard,
                    phoneNum = account.Phonenum,
                    score = iscore,
                    deliverCount = await _delivervRepository.CountAsync(e => e.VolunteerId == volunteer.AccountId)
                };

                responseList.Add(response);
                
            }

            return new VolunteerListDto
            {
                success = true,
                msg = "获取所有志愿者成功",
                response = responseList
            };
        }

        public async Task DelVolunteerAsync(string accountId,string adminId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                throw new InvalidOperationException("用户不存在");
            }
            var vol = await _volunteerRepository.GetByIdAsync(accountId);
            if (vol == null)
            {
                throw new InvalidOperationException("用户还不是志愿者，不能删除");
            }
            //从志愿者表删除
            await _volunteerRepository.DeleteByConditionAsync(v=>v.AccountId == accountId);

            //身份改变
            account.Identity = "user";
            await _accountRepository.UpdateAsync(account);

            //审核表改操作人信息、原因、时间和状态
            var applyList = await _applicationRepository.FindByConditionAsync(e => e.AccountId == accountId);
            if (applyList != null && applyList.Any())
            {
                foreach (var apply in applyList)
                {
                    var rev = await _reviewRepository.GetByIdAsync(apply.ApplicationId);
                    if (rev.Status=="通过") // 如果没有找到对应的审核记录
                    {
                        rev.AdministratorId=adminId;
                        rev.Reason = $"ID为 {adminId} 的管理员从后台删除";
                        rev.ReviewDate = DateTime.Now;
                        rev.Status = "不通过";
                        await _reviewRepository.UpdateAsync(rev);
                        break;
                    }
                }
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
