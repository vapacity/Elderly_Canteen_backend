namespace Elderly_Canteen.Data.Dtos.Volunteer
{
    public class VolunteerResponseDto
    {
        public bool success { get; set; }
        public string msg { get; set; }

        public ResponseData response { get; set; }

        public partial class ResponseData
        {
            public string accountId { get; set; } 
            public int deliverCount { get; set; } 

            public string time { get; set; } 
            public string name { get; set; }
        }
    }

    public class VolunteerListDto
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public List<ResponseData> response { get; set; } // 改为List类型

        public class ResponseData
        {
            public string accountId { get; set; }
            public string name { get; set; }
            public string idCard { get; set; }
            public string phoneNum { get; set; }
            public int deliverCount { get; set; }
            public double score { get; set; }
        }
    }
}
