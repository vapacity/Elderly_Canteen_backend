namespace Elderly_Canteen.Data.Dtos.Cart
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Request
    /// </summary>
    public partial class CartItemRequestDto
    {
        /// <summary>
        /// 购物车ID
        /// </summary>
        [JsonProperty("CART_ID")]
        public string CartId { get; set; }

        /// <summary>
        /// 菜品ID
        /// </summary>
        [JsonProperty("DISH_ID")]
        public string DishId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [JsonProperty("QUANTITY")]
        public long Quantity { get; set; }
    }
}