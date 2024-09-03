namespace Elderly_Canteen.Data.Dtos.Volunteer
{
    public class VolunteerReviewListDto
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public List<ResponseData> response { get; set; } // 改为List类型

        public class ResponseData
        {
            public string applicationId { get; set; }
            public string accountId { get; set; }
            public string applicationDate { get; set; }
        }
    }

    public class VolunteerReviewInfoDto
    {
        public bool success { get; set; }
        public string msg { get; set; }

        public ResponseData response { get; set; }

        public partial class ResponseData
        {
            public string phoneNum { get; set; } // 电话号码，必须
            public string gender { get; set; } // 性别，必须

            public string birthDate { get; set; } // 出生日期
            public string name { get; set; } 
            public string idCard { get; set; }

            public string selfStatement { get; set; }
        }

    }
}
