namespace Elderly_Canteen.Data.Dtos.Dish
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class DishRequestDto
    {
        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// ID 编号
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("recipe")]
        public List<Recipe> Recipe { get; set; }
    }

    public partial class Recipe
    {
        /// <summary>
        /// 数量
        /// </summary>
        [JsonProperty("account")]
        public double Account { get; set; }

        /// <summary>
        /// 食材ID
        /// </summary>
        [JsonProperty("IngredientId")]
        public string IngredientId { get; set; }

        /// <summary>
        /// 食材名
        /// </summary>
        [JsonProperty("IngredientName")]
        public string IngredientName { get; set; }
    }
}