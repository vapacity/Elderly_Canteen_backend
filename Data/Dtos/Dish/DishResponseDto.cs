namespace Elderly_Canteen.Data.Dtos.Dish
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Request
    /// </summary>
    public partial class DishResponseDto
    {
        [JsonProperty("dish")]
        public DishDto Dish { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public partial class DishDto
    {
        [JsonProperty("dishId")]
        public string DishId { get; set; }

        [JsonProperty("dishName")]
        public string DishName { get; set; }

        [JsonProperty("formula")]
        public List<FormulaDto> Formula { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }

    public partial class FormulaDto
    {
        /// <summary>
        /// 数量
        /// </summary>
        [JsonProperty("amount")]
        public short Amount { get; set; }

        /// <summary>
        /// 食材ID
        /// </summary>
        [JsonProperty("ingredientId")]
        public string IngredientId { get; set; }

        [JsonProperty("ingredientName")]
        public string? IngredientName { get; set; }
    }
}