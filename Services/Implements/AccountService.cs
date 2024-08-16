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

namespace Elderly_Canteen.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IConfiguration _configuration;

        public AccountService(IGenericRepository<Account> accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
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

            if (account.Password != loginRequest.password)
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
        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            //手机号重复则说明用户存在
            var existingAccount = await _accountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.Phonenum == registerRequestDto.phone);

            if (existingAccount != null)
            {
                return new RegisterResponseDto
                {
                    registerSuccess = false,
                    msg = "用户已存在",
                    timestamp=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }

            var newAccount = new Account
            {
                Accountid = await GenerateAccountIdAsync(),
                Accountname = registerRequestDto.userName,
                Password = registerRequestDto.password, // Note: You should hash the password here
                Phonenum = registerRequestDto.phone,
                Identity = "user", //一开始默认为普通用户
                Gender = registerRequestDto.gender,
                Birthdate = DateTime.TryParse(registerRequestDto.birthDate, out var birthdate) ? birthdate : (DateTime?)null,
                Portrait = registerRequestDto.avatar
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
                    portrait = account.Portrait,
                    gender = account.Gender,
                    birthDate = account.Birthdate?.ToString("yyyy-MM-dd"),
                    address = account.Address,
                    name = account.Name
                }
            };
        }

        //修改个人信息逻辑
        public async Task<PersonInfoResponseDto> AlterPersonInfoAsync(PersonInfoRequestDto personInfo, string accountId)
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
                account.Phonenum = personInfo.phoneNum;
            }

            if (!string.IsNullOrEmpty(personInfo.portrait))
            {
                account.Portrait = personInfo.portrait;
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
                else
                {
                    // 处理无法解析的情况
                    // 例如，记录错误日志或抛出异常
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
                    birthDate = account.Birthdate.ToString(),
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

        // 修改密码逻辑
        public async Task<bool> ChangePassword(string password,string accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return false;
            }
            account.Password = password;
            await _accountRepository.UpdateAsync(account);
            return true;
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
                Expires = DateTime.UtcNow.AddHours(1),
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
        
        
    }
}
