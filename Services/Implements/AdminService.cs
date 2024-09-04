using Elderly_Canteen.Data.Dtos.Account;
using Elderly_Canteen.Data.Dtos.Dish;
using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Dtos.Admin;
using Elderly_Canteen.Data.Dtos.Volunteer;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;
using Elderly_Canteen.Data.Dtos.Repository;
using Elderly_Canteen.Data.Dtos.OTP;
using Elderly_Canteen.Data.Dtos.Register;
using System.Security.Cryptography;//哈希存储

namespace Elderly_Canteen.Services.Implements
{
    public class AdminService : IAdminService
    {
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<Administrator> _adminRepository;
        private readonly IGenericRepository<Finance> _financeRepository;
        private readonly IGenericRepository<Restock> _restockRepository;
        private readonly IGenericRepository<VolReview> _reviewRepository;
        private readonly IGenericRepository<PayWage> _wageRepository;
        private readonly IConfiguration _configuration;


        public AdminService(IGenericRepository<Account> accountRepository,
                            IGenericRepository<Administrator> adminRepository,
                            IGenericRepository<Finance> financeRepository,
                            IGenericRepository<Restock> restockRepository,
                            IGenericRepository<VolReview> reviewRepository,
                            IGenericRepository<PayWage> wageRepository,
                            IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _adminRepository = adminRepository;
            _financeRepository = financeRepository;
            _restockRepository = restockRepository;
            _reviewRepository = reviewRepository;
            _wageRepository = wageRepository;
            _configuration = configuration;
        }

        //通过ID获取用户信息
        public async Task<AdminResponseDto> GetAdminByIdAsync(string id)
        {
            var admin = await _adminRepository.GetByIdAsync(id);
            if (admin == null)
            {

                return new AdminResponseDto
                {
                    Success = false,
                    Msg = $"ID为 {id} 的管理员不存在",
                    Response = null
                };
            }
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {

                return new AdminResponseDto
                {
                    Success = false,
                    Msg = $"ID为 {id} 的用户不存在",
                    Response = null
                };
            }
            return new AdminResponseDto
            {
                Success = true,
                Msg = "查找成功",
                Response = new List<AdminResponseData>
                {
                    new AdminResponseData
                    {
                        Name = account.Name,
                        IdCard = account.Idcard,
                        BirthDate = account.Birthdate?.ToString("yyyy-MM-dd"),
                        Address = account.Address,
                        Email = admin.Email,
                    }
                }
            };
        }

        //更新ID对应的用户信息
        public async Task UpdateAdminAsync(string id, AdminRequestDto admin)
        {
            var findAccount = await _accountRepository.GetByIdAsync(id);
            if (findAccount == null)
            {
                // 如果不存在，抛出一个异常或返回错误信息
                throw new InvalidOperationException($"ID为 {id} 的用户不存在。");
            }

            var findAdmin = await _adminRepository.GetByIdAsync(id);
            if (findAdmin == null)
            {
                // 如果不存在，抛出一个异常或返回错误信息
                throw new InvalidOperationException($"ID为 {id} 的管理员不存在。");
            }

            // 更新信息
            if (!string.IsNullOrEmpty(admin.PhoneNum))
            {
                // 检查数据库中是否已存在相同的手机号
                var existingAccount = await _accountRepository.GetAll()
                    .FirstOrDefaultAsync(a => a.Phonenum == admin.PhoneNum && a.Accountid != id);

                if (existingAccount != null)
                {
                    throw new InvalidOperationException("手机号已被占用");
                }
                findAccount.Phonenum = admin.PhoneNum;
            }
            if (!string.IsNullOrEmpty(admin.Position))
            {
                findAdmin.Position = admin.Position;
            }
            if (!string.IsNullOrEmpty(admin.Gender))
            {
                findAccount.Gender = admin.Gender;
            }

            // 保存更改
            await _accountRepository.UpdateAsync(findAccount);
            await _adminRepository.UpdateAsync(findAdmin);
        }

        //注册逻辑
        public async Task AddAdminAsync(AdminRegisterDto registerRequestDto)
        {
            // 检查用户是否存在
            var existingAccount = await _accountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.Phonenum == registerRequestDto.PhoneNum);

            if (existingAccount != null)
            {
                throw new InvalidOperationException("手机号已被占用");
            }
            var existingIDcard = await _accountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.Idcard == registerRequestDto.IdCard);

            if (existingIDcard != null)
            {
                throw new InvalidOperationException("该身份证号已被认证");
            }

            //处理哈希密码
            PasswordHasher hasher = new PasswordHasher();
            string hashedPassword = hasher.HashPasswordUsingSHA256("1");

            var newAccount = new Account
            {
                Accountid = await GenerateAccountIdAsync(),
                Accountname = "管理员",
                Password = hashedPassword,
                Phonenum = registerRequestDto.PhoneNum,
                Identity = "admin",
                Gender = registerRequestDto.Gender,
                Birthdate = DateTime.TryParse(registerRequestDto.BirthDate, out var birthdate) ? birthdate : (DateTime?)null,
                Address= registerRequestDto.Address,
                Name = registerRequestDto.Name,
                Idcard = registerRequestDto.IdCard,
                Money = 100,
                Portrait = "http://elderly-canteen.oss-cn-hangzhou.aliyuncs.com/16800023-portrait.jpg?Expires=1756828650&OSSAccessKeyId=LTAI5tK9KaDpnWNHxJU8hD67&Signature=inADCJChdV5U39TdwJAE7%2B7b2N8%3D"
            };
            await _accountRepository.AddAsync(newAccount);

            var newAdmin = new Administrator
            {
                AccountId = newAccount.Accountid,
                Position = registerRequestDto.Position,
                Email = registerRequestDto.Email,
            };
            await _adminRepository.AddAsync(newAdmin);
        }

        //删除ID对应的管理员  
        public async Task DeleteAdminAsync(string id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                throw new InvalidOperationException($"ID为 {id} 的用户不存在。");
            }
            var admin = await _adminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                throw new InvalidOperationException($"ID为 {id} 的管理员不存在。");
            }

            //如果管理员在进货、财务、发工资、志愿者审核表里就不能删除
            var cont1 = await _financeRepository.FindByConditionAsync(e => e.AdministratorId == id);
            var cont2 = await _restockRepository.FindByConditionAsync(e => e.AdministratorId == id);
            var cont3 = await _wageRepository.FindByConditionAsync(e => e.AdministratorId == id);
            var cont4 = await _reviewRepository.FindByConditionAsync(e => e.AdministratorId == id);
            if (cont1.Any())
            {
                throw new InvalidOperationException($"ID为 {id} 的管理员已在财务后台做过操作，不能删除。");
            }
            if (cont2.Any())
            {
                throw new InvalidOperationException($"ID为 {id} 的管理员已在进货后台做过操作，不能删除。");
            }
            if (cont3.Any())
            {
                throw new InvalidOperationException($"ID为 {id} 的管理员已在发工资后台做过操作，不能删除。");
            }
            if (cont4.Any())
            {
                throw new InvalidOperationException($"ID为 {id} 的管理员已在审核志愿者申请后台做过操作，不能删除。");
            }

            //删除管理员身份
            await _adminRepository.DeleteAsync(id);
            account.Identity = "user";
            await _accountRepository.UpdateAsync(account);
        }

        //搜索
        public async Task<AdminSearchDto> SerchAdminAsync(string? name, string? position)
        {
            List<Account> admins;
            List<Administrator> adminResults;

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(position))
            {
                // 如果 name 和 position 都为空或仅包含空白字符，返回所有用户
                adminResults = (await _adminRepository.GetAllAsync()).ToList();
                var accountIds = adminResults.Select(a => a.AccountId).ToList();
                admins = (await _accountRepository.FindByConditionAsync(u => accountIds.Contains(u.Accountid))).ToList();
            }
            else
            {
                // 根据条件从 Administrator 表中查找符合条件的记录
                adminResults = (await _adminRepository.FindByConditionAsync(a =>
                    (string.IsNullOrWhiteSpace(position) || a.Position.Contains(position)))).ToList();

                var accountIds = adminResults.Select(a => a.AccountId).ToList();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    // 如果 name 不为空，则通过 Administrator 关联的 Account 查找
                    admins = (await _accountRepository.FindByConditionAsync(u =>
                        accountIds.Contains(u.Accountid) && u.Name.Contains(name))).ToList();
                }
                else
                {
                    admins = (await _accountRepository.FindByConditionAsync(u =>
                        accountIds.Contains(u.Accountid))).ToList();
                }
            }

            // 构建返回的 AdminSearchDto
            var adminDtos = admins.Select(admin => new AdminSearchData
            {
                name = admin.Name ?? string.Empty,
                accountId = admin.Accountid,
                phoneNum = admin.Phonenum,
                position = adminResults.FirstOrDefault(a => a.AccountId == admin.Accountid)?.Position ?? string.Empty,
                gender = admin.Gender ?? string.Empty
            }).ToList();

            // 返回 AdminSearchDto
            return new AdminSearchDto
            {
                Response = adminDtos,
                Msg = "success",
                Success = true
            };
        }

        //获得个人信息逻辑
        public async Task<AdminInfoGetDto> GetAdminInfoAsync(string accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new AdminInfoGetDto
                {
                    Success = false,
                    Msg = "用户不存在",
                    Response = null
                };
            }
            var admin = await _adminRepository.GetByIdAsync(accountId);
            if (admin == null)
            {
                return new AdminInfoGetDto
                {
                    Success = false,
                    Msg = "该用户不是管理员",
                    Response = null
                };
            }

            return new AdminInfoGetDto
            {
                Success = true,
                Msg = "获取成功",
                Response = new AdminInfoData
                {
                    accountId = account.Accountid,
                    accountName = account.Accountname,
                    phoneNum = account.Phonenum,
                    portrait = account.Portrait,
                    gender = account.Gender,
                    birthDate = account.Birthdate?.ToString("yyyy-MM-dd"),
                    address = account.Address,
                    name = account.Name,
                    idCard = account.Idcard,
                    money = (double)account.Money,
                    position=admin.Position,
                    email=admin.Email
                }
            };
        }

        //修改个人信息逻辑
        public async Task AlterAdminInfoAsync(AdminInfoChangeDto personInfo, string accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                throw new InvalidOperationException($"ID为 {accountId} 的用户不存在。");
            }

            var admin = await _adminRepository.GetByIdAsync(accountId);
            if (admin == null)
            {
                throw new InvalidOperationException($"ID为 {accountId} 的管理员不存在。");
            }

            if (!string.IsNullOrEmpty(personInfo.AccountName))
            {
                account.Accountname = personInfo.AccountName;
            }

            if (!string.IsNullOrEmpty(personInfo.PhoneNum))
            {
                // 检查数据库中是否已存在相同的手机号
                var existingAccount = await _accountRepository.GetAll()
                    .FirstOrDefaultAsync(a => a.Phonenum == personInfo.PhoneNum && a.Accountid != accountId);

                if (existingAccount != null)
                {
                    throw new InvalidOperationException("手机号已被占用");
                }
                account.Phonenum = personInfo.PhoneNum;
            }

            if (!string.IsNullOrEmpty(personInfo.Gender))
            {
                account.Gender = personInfo.Gender;
            }

            if (personInfo.BirthDate != null)
            {
                if (DateTime.TryParse(personInfo.BirthDate, out DateTime birthDate))
                {
                    account.Birthdate = birthDate;
                }
            }

            if (!string.IsNullOrEmpty(personInfo.Address))
            {
                account.Address = personInfo.Address;
            }

            if (!string.IsNullOrEmpty(personInfo.Email))
            {
                var existingAdmin = await _adminRepository.GetAll()
                     .FirstOrDefaultAsync(a => a.Email == personInfo.Email && a.AccountId != accountId);

                if (existingAdmin != null)
                {
                    throw new InvalidOperationException("邮箱已被占用");
                }
                admin.Email = personInfo.Email;
            }

            await _accountRepository.UpdateAsync(account);
            await _adminRepository.UpdateAsync(admin);
        }







        private async Task<string> GenerateAccountIdAsync()
        {
            const string prefix = "168";

            // Get the maximum existing AccountId that starts with the prefix
            var maxAccountId = await _accountRepository.GetAll()
                .Where(a => a.Accountid.StartsWith(prefix))
                .OrderByDescending(a => a.Accountid)
                .Select(a => a.Accountid)
                .FirstOrDefaultAsync();

            if (maxAccountId == null)
            {
                // If no account with the prefix exists, start from the first possible ID
                return prefix + "00001";
            }

            // Extract the numerical part and increment it by 1
            var numericalPart = int.Parse(maxAccountId.Substring(prefix.Length));
            var newAccountId = numericalPart + 1;

            // Format the new account ID with leading zeros
            return prefix + newAccountId.ToString("D5");
        }

        public class PasswordHasher
        {
            // 使用 MD5 哈希密码
            public string HashPasswordUsingMD5(string password)
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    // 转换为16进制字符串
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    return sb.ToString(); // 生成的哈希字符串为32个字符
                }
            }

            // 使用 SHA256 并截断前20个字符
            public string HashPasswordUsingSHA256(string password)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);

                    // 转换为16进制字符串
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    return sb.ToString().Substring(0, 20); // 截断为20个字符
                }
            }
        }
    }
}
