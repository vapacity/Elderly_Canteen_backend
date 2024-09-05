namespace Elderly_Canteen.Data.Dtos.Order
{
    public class IdentityResponseDto
    {
        public bool success {  get; set; }
        public string msg { get; set; }
        public IdentityDto response { get; set; }
    }
    public class IdentityDto
    {
        public bool isDeliver { get; set; }
        public bool isOwner { get; set; }
    }
}
