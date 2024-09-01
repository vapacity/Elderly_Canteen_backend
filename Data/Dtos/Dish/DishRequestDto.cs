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
    public partial class DishRequestDto
    {
        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty("cateId")]
        public string CateId { get; set; }

        [JsonProperty("formula")]
        public List<FormulaDto> Formula { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("dishId")]
        public string? DishId { get; set; }
    }

}