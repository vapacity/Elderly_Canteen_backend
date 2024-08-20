namespace Elderly_Canteen.Data.Dtos.Register
{
    public class RegisterRequestDto
    {
        public string userName { set; get; }
        public string password { set;get; }
        public string phone {  set;get; }
        public string verificationCode { set; get; }
        public string gender { set; get; }
        public string? birthDate { set;get; }
    }
}
