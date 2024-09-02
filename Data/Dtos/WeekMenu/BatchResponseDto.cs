namespace Elderly_Canteen.Data.Dtos.WeekMenu
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Request
    /// </summary>
    public partial class BatchResponseDto
    {
        [JsonProperty("discountDishes")]
        public List<DiscountDish> DiscountDishes { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public partial class DiscountDish
    {
        [JsonProperty("discountPrice")]
        public decimal DiscountPrice { get; set; }

        [JsonProperty("dishId")]
        public string DishId { get; set; }

        [JsonProperty("dishName")]
        public string DishName { get; set; }
    }
}