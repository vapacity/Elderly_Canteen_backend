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

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var account = await _accountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.PhoneNum == loginRequest.PhoneNum);

            if (account == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Msg = "Account does not exist"
                };
            }

            if (account.Password != loginRequest.Password)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Msg = "Incorrect password"
                };
            }

            var token = GenerateJwtToken(account);
            return new LoginResponseDto
            {
                Success = true,
                Msg = "Login successful",
                Response = new LoginResponseDto.ResponseData
                {
                    Token = token,
                    Role = account.Identity,
                    Username = account.AccountName,
                    Account_id = account.AccountId
                }
            };
        }
        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            //手机号重复则说明用户存在
            var existingAccount = await _accountRepository.GetAll()
                .FirstOrDefaultAsync(a => a.PhoneNum == registerRequestDto.Phone);

            if (existingAccount != null)
            {
                return new RegisterResponseDto
                {
                    Msg = "用户已存在",
                    Timestamp=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }

            var newAccount = new Account
            {
                AccountId = await GenerateAccountIdAsync(),
                AccountName = registerRequestDto.Username,
                Password = registerRequestDto.Password, // Note: You should hash the password here
                PhoneNum = registerRequestDto.Phone,
                Identity = "user", //一开始默认为普通用户
                Gender = registerRequestDto.Gender,
                BirthDate = DateTime.TryParse(registerRequestDto.Birthdate, out var birthdate) ? birthdate : (DateTime?)null,
                Portrait = registerRequestDto.Avatar
            };

            await _accountRepository.AddAsync(newAccount);
            return new RegisterResponseDto
            {
                Msg = "注册成功"

            };
        }
        public async Task<PersonInfoResponseDto> GetPersonInfoAsync(string account_id)
        {
            var account = await _accountRepository.GetByIdAsync(account_id);
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
                    accountId = account.AccountId,
                    accountName = account.AccountName,
                    phoneNum = account.PhoneNum,
                    identity = account.Identity,
                    portrait = account.Portrait,
                    gender = account.Gender,
                    birthDate = account.BirthDate?.ToString("yyyy-MM-dd"),
                    address = account.Address,
                    name = account.Name
                }
            };
        }
        public async Task<PersonInfoResponseDto> AlterPersonInfoAsync(PersonInfoRequestDto personInfo,string account_id)
        {
            var account = await _accountRepository.GetByIdAsync(account_id);
            if(account == null)
            {
                return new PersonInfoResponseDto
                { 
                    getSuccess=false,
                    msg = "用户不存在",
                    response = null
                };
            }
            account.AccountName = personInfo.accountName;
            account.PhoneNum = personInfo.phoneNum;
            account.Portrait = personInfo.portrait;
            account.Gender = personInfo.gender;

            if (personInfo.birthDate != null)
            {
                if (DateTime.TryParse(personInfo.birthDate, out DateTime birthDate))
                {
                    account.BirthDate = birthDate;
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
                    accountId = account.AccountId,
                    accountName = account.AccountName,
                    phoneNum = account.PhoneNum,
                    identity = account.Identity,
                    portrait = account.Portrait,
                    gender = account.Gender,
                    birthDate = account.BirthDate.ToString(),
                    address = account.Address,
                    name = account.Name
                }
            };
        }
        /*以下为辅助用工具函数，我建议另写一个tools类来存放所有的工具函数，暂时感觉必要性不大，很难复用*/
        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, account.AccountId),
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
                .Where(a => a.AccountId.StartsWith(prefix))
                .OrderByDescending(a => a.AccountId)
                .Select(a => a.AccountId)
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
        public async Task<List<AccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();

            var responseList = new List<AccountDto>();

            foreach (var account in accounts)
            {
                var response = new AccountDto
                {
                    ACCOUNT_ID = account.AccountId,
                    ACCOUNT_NAME = account.AccountName,
                    PHONE_NUM = account.PhoneNum,
                    IDENTITY = account.Identity,
                    PORTRAIT = account.Portrait,
                    GENDER = account.Gender,
                    PassWord = account.Password
                };

                responseList.Add(response);
            }

            return responseList;
        }
    }
}
