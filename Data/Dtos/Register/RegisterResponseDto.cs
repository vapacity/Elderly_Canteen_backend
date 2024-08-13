namespace Elderly_Canteen.Data.Dtos.Register
{
    public class RegisterResponseDto
    {
        public bool registerSuccess { get; set; }
        public string msg { get; set; }
        public string? timestamp { get; set; }
        public RegisterResponse response { get; set; }
        

    }
    public class RegisterResponse
    {
        public string token { get; set; }
        public string identity { get; set; }
        public string accountName { get; set; }
        public string accountId { get; set; }
    }

}
