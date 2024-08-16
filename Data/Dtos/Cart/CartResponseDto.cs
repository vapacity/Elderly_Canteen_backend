namespace Elderly_Canteen.Data.Dtos.Cart
{
    public class CartResponseDto
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public CartResponse response { get; set; }

        public class CartResponse
        {
            public string cartId { get; set; }
            public DateTime? createTime { get; set; }
            public DateTime? updateTime { get; set; }
        }
    }
}
