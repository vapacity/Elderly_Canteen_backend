namespace Elderly_Canteen.Data.Dtos.Register
{
    public class RegisterRequestDto
    {
        public List<UserRegistrationDto> Users { get; set; } = new List<UserRegistrationDto>();
    }

    public class UserRegistrationDto
    {
        public string userName { set; get; }
        public string password { set; get; }
        public string phone { set; get; }
        public string gender { set; get; }
        public string? birthDate { set; get; }
        public string address { set; get; }
        public string? name { set; get; }
        public string? idCard { set; get; }
    }
}
