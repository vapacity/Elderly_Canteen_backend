namespace Elderly_Canteen.Data.Dtos.AuthenticationDto
{
    public class AuthenticationRequestDto
    {
        public List<AuthenticationDto> Vols { get; set; } = new List<AuthenticationDto>();
    }


    public class AuthenticationDto
    {
        public string accountId { set; get; }
        public string name { get; set; }
        public string idCard { get; set; }
    }
}
