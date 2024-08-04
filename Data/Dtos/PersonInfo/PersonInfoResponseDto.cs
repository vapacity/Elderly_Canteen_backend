namespace Elderly_Canteen.Data.Dtos.PersonInfo
{
    public class PersonInfoResponseDto
    {
        public bool? alterSuccess { get; set; }// 改变情况
        public bool? getSuccess { get; set; } // 状态，必须
        public string msg { get; set; } // 提示信息，必须
        public ResponseData response { get; set; } // 响应数据，必须
    }

    public class ResponseData
    {
        // 必须字段
        public string accountId { get; set; } // 账号ID，必须
        public string accountName { get; set; } // 账号名称，必须
        public string phoneNum { get; set; } // 电话号码，必须
        public string identity { get; set; } // 身份类型，必须
        public string portrait { get; set; } // 头像URL，必须
        public string gender { get; set; } // 性别，必须

        // 非必须字段
        public string? birthDate { get; set; } // 出生日期，可选
        public string? address { get; set; } // 地址，可选
        public string? name { get; set; } // 姓名，可选
        public string? password {  get; set; }
    }
}