using Elderly_Canteen.Data.Dtos.EmployeeInfo;
using Elderly_Canteen.Data.Dtos.PersonInfo;
using Elderly_Canteen.Data.Dtos.Users;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Text;

namespace Elderly_Canteen.Services.Implements
{
    public class UsersService : IUsersService
    {
        private readonly IGenericRepository<Account> _usersRepository;
        private readonly IGenericRepository<Finance> _financeRepository;
        private readonly IGenericRepository<Donation> _donationRepository;
        private readonly IConfiguration _configuration;

        public UsersService(IGenericRepository<Account> usersRepository,
                            IGenericRepository<Finance> financeRepository,
                            IGenericRepository<Donation> donationRepository, 
                            IConfiguration configuration)
        {
            _usersRepository = usersRepository;
            _financeRepository = financeRepository;
            _donationRepository = donationRepository;
            _configuration = configuration;
        }

        //通过ID获取用户信息
        public async Task<UsersResponseDto> GetUserByIdAsync(string id)
        {
            var user = await _usersRepository.GetByIdAsync(id);
            if (user == null)
            {

                return new UsersResponseDto
                {
                    Success = false,
                    Msg = $"ID为 {id} 的用户不存在",
                    Response = null
                };
            }

            return new UsersResponseDto
            {
                Success = true,
                Msg = "查找成功",
                Response = new List<UsersResponseData>
                {
                    new UsersResponseData
                    {
                        Name = user.Name,
                        IdCard = user.Idcard,
                        BirthDate = user.Birthdate?.ToString("yyyy-MM-dd"),
                        Address = user.Address,
                    }
                }
            };
        }

        //更新ID对应的用户信息
        public async Task UpdateUserAsync(string id, UsersRequestDto user)
        {
            var findUser = await _usersRepository.FindByConditionAsync(e => e.Accountid == id);

            if (findUser == null || !findUser.Any())
            {
                // 如果不存在，抛出一个异常或返回错误信息
                throw new InvalidOperationException($"ID为 {id} 的用户不存在。");
            }
            // 获取找到的第一个实体
            var userToModify = findUser.First();

            // 更新信息
            if (!string.IsNullOrEmpty(user.AccountName))
            {
                userToModify.Accountname = user.AccountName;
            }

            if (!string.IsNullOrEmpty(user.PhoneNum))
            {
                // 检查数据库中是否已存在相同的手机号
                var existingAccount = await _usersRepository.GetAll()
                    .FirstOrDefaultAsync(a => a.Phonenum == user.PhoneNum && a.Accountid != id);

                if (existingAccount != null)
                {
                    throw new InvalidOperationException("手机号已被占用");
                }
                userToModify.Phonenum = user.PhoneNum;
            }
            if (!string.IsNullOrEmpty(user.Identity))
            {
                userToModify.Identity = user.Identity;
            }
            if (!string.IsNullOrEmpty(user.Identity))
            {
                userToModify.Gender = user.Gender;
            }

            // 保存更改
            await _usersRepository.UpdateAsync(userToModify);
        }

        //删除ID对应的用户
        public async Task DeleteUserAsync(string id)
        {
            var account = await _usersRepository.GetByIdAsync(id);

            if (account == null)
            {
                throw new InvalidOperationException($"ID为 {id} 的用户不存在。");
            }
            // 如果注销账户在财务记录中，更新财务记录，将 accountId 设置为特殊值
            await _financeRepository.UpdateAsync(
                f => f.AccountId == id,
                f => f.AccountId = "DELETED");
            await _donationRepository.UpdateAsync(
                f => f.AccountId == id,
                f => f.AccountId = "DELETED");

            await _usersRepository.DeleteAsync(id);
        }

        public async Task<string> CreatePsdAsync(int length, int maxLength)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_~";
            const string numbers = "0123456789";
            const string upperCaseLetters = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            const string specialChars = "!@#$%^&*?_~";

            int finalLength = Math.Min(length, maxLength);

            var random = new Random();
            var password = new StringBuilder();

            // 确保密码包含至少一个数字、一个大写字母和一个特殊字符
            password.Append(numbers[random.Next(numbers.Length)]);
            password.Append(upperCaseLetters[random.Next(upperCaseLetters.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            // 补充剩余的字符
            while (password.Length < finalLength)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            // 打乱字符顺序
            return new string(password.ToString().OrderBy(x => random.Next()).ToArray());
        }
    }
}
