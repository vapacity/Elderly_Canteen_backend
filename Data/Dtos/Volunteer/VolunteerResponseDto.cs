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
            public double deliverCount { get; set; } 

            public string time { get; set; } 
            public string name { get; set; }
        }
    }
}
