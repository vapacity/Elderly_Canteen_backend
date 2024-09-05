namespace Elderly_Canteen.Data.Dtos.VolServe
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public partial class AccessOrderResponseDto
    {
        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("response")]
        public List<Response> Response { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("ORDER_ID")]
        public string OrderId { get; set; }

        [JsonProperty("CUS_ADDRESS")]
        public string CusAddress { get; set; }

        [JsonProperty("DELIVER_OR_DINING")]
        public bool DeliverOrDining { get; set; }

        [JsonProperty("DELIVER_STATUS")]
        public string DeliverStatus { get; set; }

        [JsonProperty("MONEY")]
        public decimal Money { get; set; }

        [JsonProperty("STATUS")]
        public string Status { get; set; }

        [JsonProperty("REMARK", NullValueHandling = NullValueHandling.Ignore)]
        public string Remark { get; set; }

        [JsonProperty("subsidy")]
        public decimal Subsidy { get; set; }

        [JsonProperty("UPDATED_TIME")]
        public DateTime UpdatedTime { get; set; }

        [JsonProperty("orderDishes")]
        public List<OrderDish> OrderDishes { get; set; }
    }

    public partial class OrderDish
    {
        [JsonProperty("DISH_NAME")]
        public string DishName { get; set; }

        [JsonProperty("PICTURE", NullValueHandling = NullValueHandling.Ignore)]
        public string Picture { get; set; }

        [JsonProperty("PRICE")]
        public decimal Price { get; set; }

        [JsonProperty("QUANTITY")]
        public int Quantity { get; set; }
    }
}
