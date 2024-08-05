namespace Elderly_Canteen.Data.Dtos.Login
{
    public class LoginResponseDto
    {
        public bool loginSuccess { get; set; }
        public string msg { get; set; }
        public ResponseData response { get; set; }

        public class ResponseData
        {
            public string token { get; set; }
            public string identity { get; set; }
            public string accountName { get; set; }
            public string accountId {  get; set; }
        }
    }
}
