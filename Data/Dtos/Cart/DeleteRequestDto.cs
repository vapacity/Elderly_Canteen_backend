namespace Elderly_Canteen.Data.Dtos.Cart
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class DeleteRequestDto
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
    }
}
