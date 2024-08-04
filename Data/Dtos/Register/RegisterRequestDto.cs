namespace Elderly_Canteen.Data.Dtos.Register
{
    public class RegisterRequestDto
    {
        public string Username { set; get; }
        public string Password { set;get; }
        public string Phone {  set;get; }
        public string VerificationCode { set; get; }
        public string Gender { set; get; }
        public string? Birthdate { set;get; }
        public string? Avatar { set;get; }
    }
}
