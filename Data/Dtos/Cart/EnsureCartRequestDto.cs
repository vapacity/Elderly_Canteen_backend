namespace Elderly_Canteen.Data.Dtos.Cart
{
    public class EnsureCartRequestDto
    {
        public string CART_ID { get; set; }
        public string? newAddress { get; set; }
        public bool deliver_or_dining { get; set; }
        public string? remark { get; set; }
    }
}
