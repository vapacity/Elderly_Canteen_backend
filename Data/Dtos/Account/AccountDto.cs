namespace Elderly_Canteen.Data.Dtos.Account
{
    public class AccountDto
    {
        // 必须字段
        public string ACCOUNT_ID { get; set; } // 账号ID，必须
        public string ACCOUNT_NAME { get; set; } // 账号名称，必须
        public string PHONE_NUM { get; set; } // 电话号码，必须
        public string IDENTITY { get; set; } // 身份类型，必须
        public string PORTRAIT { get; set; } // 头像URL，必须
        public string GENDER { get; set; } // 性别，必须

        // 非必须字段
        public string? BIRTH_DATE { get; set; } // 出生日期，可选
        public string? ADDRESS { get; set; } // 地址，可选
        public string? NAME { get; set; } // 姓名，可选
        public string? PassWord { get; set; }
    }
}
