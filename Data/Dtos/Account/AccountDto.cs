namespace Elderly_Canteen.Data.Dtos.Account
{
    public class AccountDto
    {
        // 必须字段
        public string accountId { get; set; } // 账号ID，必须
        public string accountName { get; set; } // 账号名称，必须
        public string phoneNum { get; set; } // 电话号码，必须
        public string identity { get; set; } // 身份类型，必须
        public string portrait { get; set; } // 头像URL，必须
        public string gender { get; set; } // 性别，必须
        public decimal money { get; set; }
        // 非必须字段
        public DateTime? birthDate { get; set; } // 出生日期，可选
        public string? address { get; set; } // 地址，可选
        public string? name { get; set; } // 姓名，可选
        public string? password { get; set; }
        public string? Idcard { get; set; }
    }
}
