namespace Elderly_Canteen.Data.Dtos.Login
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public ResponseData Response { get; set; }

        public class ResponseData
        {
            public string Token { get; set; }
            public string Role { get; set; }
            public string Username { get; set; }
            public string Account_id {  get; set; }
        }
    }
}
