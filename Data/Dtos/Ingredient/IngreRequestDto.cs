namespace Elderly_Canteen.Data.Dtos.Ingredient
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Request
    /// </summary>
    public partial class IngreRequestDto
    {
        [JsonProperty("IngredientId")]
        public string? IngredientId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty("IngredientName")]
        public string? IngredientName { get; set; }
    }
}