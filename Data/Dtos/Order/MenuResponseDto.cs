namespace Elderly_Canteen.Data.Dtos.Order
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Request
    /// </summary>
    public partial class MenuResponseDto
    {
        [JsonProperty("menu")]
        public List<Menu> Menu { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public partial class Menu
    {
        [JsonProperty("discountPrice")]
        public decimal DiscountPrice { get; set; }

        [JsonProperty("dishId")]
        public string DishId { get; set; }

        [JsonProperty("dishName")]
        public string DishName { get; set; }

        [JsonProperty("dishPrice")]
        public decimal DishPrice { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("sales")]
        public int Sales { get; set; }

        [JsonProperty("stock")]
        public int Stock { get; set; }
    }
}