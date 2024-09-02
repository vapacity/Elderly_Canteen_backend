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
using System.Security.Principal;
using Elderly_Canteen.Tools;
using Elderly_Canteen.Data.Dtos.OTP;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;//哈希存储


namespace Elderly_Canteen.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IGenericRepository<Account> _accountRepository;
        //这是为了注销用户的时候不把财务信息删除掉，因此把该账户的财务记录的id改为“DELETED”，其他的应该级联删除
        private readonly IGenericRepository<Finance> _financeRepository;
        private readonly IGenericRepository<Donation> _donationRepository;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly IWebHostEnvironment _environment;

        public AccountService(IGenericRepository<Account> accountRepository,
                            IGenericRepository<Finance> financeRepository,
                            IGenericRepository<Donation> donationRepository,
                            IConfiguration configuration,
                            IMemoryCache memoryCache,
                            IWebHostEnvironment environment)
        {
            _accountRepository = accountRepository;
            _financeRepository = financeRepository;
            _donationRepository = donationRepository;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _environment = environment;
        }

        //发送验证码逻辑
        public async Task<GetOTPResponseDto> SendOTPAsync(GetOTPRequestDto request)
        {
            var response = new GetOTPResponseDto();
            var accessKeyId = "LTAI5tK1wVEksEXtvyRf6V9H";
            var accessKeySecret = "LDNuw6ekoZXTqLtG3PoxfHQgnvqC1Q";
            var signName = "长者食堂";
            var templateCode = "SMS_471945131";

            try
            {
                // 实例化 AliSmsSender
                var smsSender = new AliSmsSender(accessKeyId, accessKeySecret, signName);
                // 生成随机四位数验证码
                var random = new Random();
                var code = random.Next(1000, 10000).ToString();

                // 保存验证码到数据库或缓存中（此处可以扩展）
                await SaveOTPAsync(request.PhoneNum, code);

                // 发送短信验证码
                var result = await smsSender.SendAsync(request.PhoneNum, templateCode, new { code = code });


                if (result.Code == "OK")
                {
                    response.Success = true;
                    response.Msg = "验证码已发送";
                }
                else
                {
                    response.Success = false;
                    response.Msg = "发送失败：" + result.Message;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Msg = "发送验证码时发生错误：" + ex.Message;
            }

            return response;

        }

        //保存验证码逻辑(保存到缓存)
        private async Task SaveOTPAsync(string phoneNum, string code)
        {
            // 将验证码保存到内存缓存中，设置3分钟的过期时间
            _memoryCache.Set(phoneNum, code, TimeSpan.FromMinutes(3));
        }
        //获取保存的验证码逻辑
        private async Task<string> GetOTPAsync(string phoneNum)
        {
            // 从内存缓存中获取验证码
            _memoryCache.TryGetValue(phoneNum, out string code);
            return code;
        }

        //验证码登录
        public async Task<VerifyOTPResponseDto<OTPLoginResponseDto>> VerifyLoginOTPAsync(VerifyOTPRequestDto request)
        {
            var response = new VerifyOTPResponseDto<OTPLoginResponseDto>();
            // 从缓存中获取验证码
            var savedCode = await GetOTPAsync(request.PhoneNum);
            if (savedCode == null || savedCode != request.Code)
            {
                response.Success = false;
                response.Msg = "验证码无效或已过期";
                return response;
            }
            // 验证通过，查找用户
            var account = await _accountRepository.GetAll()
               .FirstOrDefaultAsync(a => a.Phonenum == request.PhoneNum);
            if (account == null)
            {
                response.Success = false;
                response.Msg = "用户不存在！";
                return response;
            }
            //生成 Token 并返回用户信息
            var token = GenerateJwtToken(account);
            response.Success = true;
            response.Msg = "登录成功";
            response.Response = new OTPLoginResponseDto
            {
                Token = token,
                Identity = account.Identity,
                AccountName = account.Accountname,
                AccountId = account.Accountid
            };
            // 验证通过，删除缓存中的验证码
            _memoryCache.Remove(request.PhoneNum);
            return response;
        }
        //验证码验证
        public async Task<VerifyOTPResponseDto<OTPLoginResponseDto>> VerifyOTPWithoutUserCheckAsync(VerifyOTPRequestDto request)
        {
            var response = new VerifyOTPResponseDto<OTPLoginResponseDto>();
            // 从缓存中获取验证码
            var savedCode = await GetOTPAsync(request.PhoneNum);
            if (savedCode == null || savedCode != request.Code)
            {
                response.Success = false;
                response.Msg = "验证码无效或已过期";
                return response;
            }
            response.Success = true;
            response.Msg = "验证成功";
            // 验证通过，删除缓存中的验证码
            _memoryCache.Remove(request.PhoneNum);
            return response;
        }

        //登录逻辑
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var account = await _accountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.Phonenum == loginRequest.phoneNum);

            if (account == null)
            {
                return new LoginResponseDto
                {
                    loginSuccess = false,
                    msg = "Account does not exist"
                };
            }

            PasswordHasher hasher = new PasswordHasher();
            string inputHashedPassword = hasher.HashPasswordUsingSHA256(loginRequest.password);
            if (inputHashedPassword != account.Password)
            {
                return new LoginResponseDto
                {
                    loginSuccess = false,
                    msg = "Incorrect password"
                };
            }

            var token = GenerateJwtToken(account);
            return new LoginResponseDto
            {
                loginSuccess = true,
                msg = "Login successful",
                response = new LoginResponseDto.ResponseData
                {
                    token = token,
                    identity = account.Identity,
                    accountName = account.Accountname,
                    accountId = account.Accountid
                }
            };
        }

        //注册逻辑
        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto, IFormFile avatar)
        {
            // 验证验证码
            var verifyOtpResult = await VerifyOTPWithoutUserCheckAsync(new VerifyOTPRequestDto
            {
                PhoneNum = registerRequestDto.phone,
                Code = registerRequestDto.verificationCode 
            });

            if (!verifyOtpResult.Success)
            {
                return new RegisterResponseDto
                {
                    registerSuccess = false,
                    msg = verifyOtpResult.Msg,
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }

            // 检查用户是否存在
            var existingAccount = await _accountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.Phonenum == registerRequestDto.phone);

            if (existingAccount != null)
            {
                return new RegisterResponseDto
                {
                    registerSuccess = false,
                    msg = "用户已存在",
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }

            // 处理头像文件并生成路径
            string avatarPath = null;
            if (avatar != null)
            {
                avatarPath = await SaveAvatarAsync(avatar);
            }

            //处理哈希密码
            PasswordHasher hasher = new PasswordHasher();
            string hashedPassword = hasher.HashPasswordUsingSHA256(registerRequestDto.password);

            var newAccount = new Account
            {
                Accountid = await GenerateAccountIdAsync(),
                Accountname = registerRequestDto.userName,
                Password = hashedPassword,
                Phonenum = registerRequestDto.phone,
                Identity = "user",
                Gender = registerRequestDto.gender,
                Birthdate = DateTime.TryParse(registerRequestDto.birthDate, out var birthdate) ? birthdate : (DateTime?)null,
                Portrait = avatarPath // 保存头像路径
            };

            await _accountRepository.AddAsync(newAccount);
            var token = GenerateJwtToken(newAccount);
            return new RegisterResponseDto
            {
                registerSuccess = true,
                msg = "注册成功",
                response = new RegisterResponse
                {
                    token = token,
                    identity = "user",
                    accountName = newAccount.Accountname,
                    accountId = newAccount.Accountid,
                }
            };
        }


        //获得个人信息逻辑
        public async Task<PersonInfoResponseDto> GetPersonInfoAsync(string accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new PersonInfoResponseDto
                {
                    getSuccess = false,
                    msg = "用户不存在",
                    response = null
                };
            }

            return new PersonInfoResponseDto
            {
                getSuccess = true,
                msg = "获取成功",
                response = new ResponseData
                {
                    accountId = account.Accountid,
                    accountName = account.Accountname,
                    phoneNum = account.Phonenum,
                    identity = account.Identity,
                    portrait = account.Portrait != null ? $"/{account.Portrait}" : null, // 返回相对路径
                    gender = account.Gender,
                    birthDate = account.Birthdate?.ToString("yyyy-MM-dd"),
                    address = account.Address,
                    name = account.Name
                }
            };
        }

        //修改个人信息逻辑
        public async Task<PersonInfoResponseDto> AlterPersonInfoAsync(PersonInfoRequestDto personInfo, string accountId, IFormFile avatar)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new PersonInfoResponseDto
                {
                    alterSuccess = false,
                    msg = "用户不存在",
                    response = null
                };
            }

            if (!string.IsNullOrEmpty(personInfo.accountName))
            {
                account.Accountname = personInfo.accountName;
            }

            if (!string.IsNullOrEmpty(personInfo.phoneNum))
            {
                // 检查数据库中是否已存在相同的手机号
                var existingAccount = await _accountRepository.GetAll()
                    .FirstOrDefaultAsync(a => a.Phonenum == personInfo.phoneNum && a.Accountid != accountId);

                if (existingAccount != null)
                {
                    return new PersonInfoResponseDto
                    {
                        alterSuccess = false,
                        msg = "手机号已被占用",
                        response = null
                    };
                }
                account.Phonenum = personInfo.phoneNum;
            }

            if (avatar != null)
            {
                // 处理头像文件并生成路径
                var avatarPath = await SaveAvatarAsync(avatar);
                account.Portrait = avatarPath;
            }

            if (!string.IsNullOrEmpty(personInfo.gender))
            {
                account.Gender = personInfo.gender;
            }

            if (personInfo.birthDate != null)
            {
                if (DateTime.TryParse(personInfo.birthDate, out DateTime birthDate))
                {
                    account.Birthdate = birthDate;
                }
            }

            if (!string.IsNullOrEmpty(personInfo.address))
            {
                account.Address = personInfo.address;
            }

            if (!string.IsNullOrEmpty(personInfo.name))
            {
                account.Name = personInfo.name;
            }

            await _accountRepository.UpdateAsync(account);

            return new PersonInfoResponseDto
            {
                alterSuccess = true,
                msg = "修改成功",
                response = new ResponseData
                {
                    accountId = account.Accountid,
                    accountName = account.Accountname,
                    phoneNum = account.Phonenum,
                    identity = account.Identity,
                    portrait = account.Portrait,
                    gender = account.Gender,
                    birthDate = account.Birthdate?.ToString("yyyy-MM-dd"),
                    address = account.Address,
                    name = account.Name
                }
            };
        }



        //获得所有个人信息逻辑
        public async Task<List<AccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();

            var responseList = new List<AccountDto>();

            foreach (var account in accounts)
            {
                if (account.Accountid != "DELETED")
                {
                    var response = new AccountDto
                    {
                        accountId = account.Accountid,
                        accountName = account.Accountname,
                        phoneNum = account.Phonenum,
                        identity = account.Identity,
                        portrait = account.Portrait,
                        gender = account.Gender,
                        password = account.Password,
                        address = account.Address,
                        name = account.Name,
                        Idcard = account.Idcard

                    };

                    responseList.Add(response);
                }
            }

            return responseList;
        }

        //实名认证逻辑
        public async Task<AuthenticationResponseDto> NameAuthentication(AuthenticationRequestDto input,string accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new AuthenticationResponseDto
                {
                    success = false,
                    msg = "账户不存在"
                };
            }
            if (account.Idcard != null)
            {
                return new AuthenticationResponseDto
                {
                    success = false,
                    msg = "已实名认证"
                };
            }
            var existedIdCard = await _accountRepository.FindByConditionAsync(account => account.Idcard == input.idCard);
            if (existedIdCard.Any())
            {
                return new AuthenticationResponseDto
                {
                    success = false,
                    msg = "该身份已被注册"
                };
            }
            account.Name = input.name;
            account.Idcard = input.idCard;
            await _accountRepository.UpdateAsync(account);
            return new AuthenticationResponseDto
            {
                success = true,
                msg = "认证成功"
            };
        }

        // 改绑手机
        public async Task<PhoneResponseDto> ChangePhone(PhoneRequestDto request, string accountId)
        {
            // 检查数据库中是否已存在相同的手机号
            var existingAccount = await _accountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.Phonenum == request.NewPhoneNum && a.Accountid != accountId);

            if (existingAccount != null)
            {
                return new PhoneResponseDto
                {
                    success = false,
                    msg = "手机号已被占用",
                };
            }
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new PhoneResponseDto
                {
                    success = false,
                    msg = "用户验证失败",
                };
            }
            account.Phonenum = request.NewPhoneNum;
            await _accountRepository.UpdateAsync(account);
            return new PhoneResponseDto
            {
                success = true,
                msg = "改绑成功",
            };
        }

        // 验证用户输入的旧密码是否正确
        public async Task<bool> VerifyPassword(string oldPassword, string accountId)
        {
            // 从数据库获取用户存储的密码
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return false;
            }

            // 比较旧密码
            PasswordHasher hasher = new PasswordHasher();
            string inputHashedPassword = hasher.HashPasswordUsingSHA256(oldPassword);
            return inputHashedPassword == account.Password;
        }

        // 修改密码逻辑
        public async Task<bool> ChangePassword(string password,string accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return false;
            }
            PasswordHasher hasher = new PasswordHasher();
            string hashedPassword = hasher.HashPasswordUsingSHA256(password);
            account.Password = hashedPassword;
            await _accountRepository.UpdateAsync(account);
            return true;
        }

        //注销账户逻辑
        public async Task<bool> DeleteAccountAsync(string accountId)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(accountId);

                if (account == null)
                {
                    return false;
                }
                // 如果注销账户在财务记录中，更新财务记录，将 accountId 设置为特殊值
                await _financeRepository.UpdateAsync(
                    f => f.AccountId == accountId,
                    f => f.AccountId = "DELETED");
                await _donationRepository.UpdateAsync(
                    f => f.AccountId == accountId,
                    f => f.AccountId = "DELETED");

                await _accountRepository.DeleteAsync(accountId);

                return true;
            }
            catch
            {
                // 这里可以添加日志记录以便调试
                return false;
            }
        }






        //以下为辅助用工具函数，我建议另写一个tools类来存放所有的工具函数，暂时感觉必要性不大，很难复用
        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, account.Accountid),
                    new Claim(ClaimTypes.Role, account.Identity),
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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

        private async Task<string> SaveAvatarAsync(IFormFile avatar)
        {
            // 使用 ContentRootPath 来获取项目根目录路径
            var uploadPath = Path.Combine(_environment.ContentRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(avatar.FileName)}_{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            return Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
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
