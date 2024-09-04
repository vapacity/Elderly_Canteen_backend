namespace Elderly_Canteen.Data.Dtos.Order
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class OrderInfoDto
    {
        /// <summary>
        /// 提示
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }

        /// <summary>
        /// 响应
        /// </summary>
        [JsonProperty("response")]
        public OrderItem Response { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    /// <summary>
    /// 响应
    /// </summary>
    public partial class OrderItem
    {
        [JsonProperty("ORDER_ID")]
        public string OrderId { get; set; }
        /// <summary>
        /// 客户地址
        /// </summary>
        [JsonProperty("CUS_ADDRESS", NullValueHandling = NullValueHandling.Ignore)]
        public string CusAddress { get; set; }

        /// <summary>
        /// true表示外送 false表示堂食
        /// </summary>
        [JsonProperty("DELIVER_OR_DINING")]
        public bool DeliverOrDining { get; set; }

        /// <summary>
        /// 配送状态
        /// </summary>
        [JsonProperty("DELIVER_STATUS")]
        public string DeliverStatus { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [JsonProperty("MONEY")]
        public decimal Money { get; set; }

        /// <summary>
        /// 订单餐品
        /// </summary>
        [JsonProperty("orderDishes")]
        public List<OrderDish> OrderDishes { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [JsonProperty("REMARK", NullValueHandling = NullValueHandling.Ignore)]
        public string Remark { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [JsonProperty("STATUS")]
        public string Status { get; set; }

        /// <summary>
        /// 补贴
        /// </summary>
        [JsonProperty("subsidy")]
        public decimal Subsidy { get; set; }

        [JsonProperty("UPDATED_TIME")]
        public string UpdatedTime { get; set; }
    }

    public partial class OrderDish
    {
        /// <summary>
        /// 菜品名称
        /// </summary>
        [JsonProperty("DISH_NAME")]
        public string DishName { get; set; }

        /// <summary>
        /// 图片url
        /// </summary>
        [JsonProperty("PICTURE", NullValueHandling = NullValueHandling.Ignore)]
        public string Picture { get; set; }

        /// <summary>
        /// 菜品价格
        /// </summary>
        [JsonProperty("PRICE")]
        public decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [JsonProperty("QUANTITY")]
        public int Quantity { get; set; }
    }
}