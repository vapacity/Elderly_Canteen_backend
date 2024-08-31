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
    public partial class AllIngreResponseDto
    {
        [JsonProperty("ingredients")]
        public List<IngredientDto> Ingredients { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    /// <summary>
    /// ingredient
    /// </summary>
    public partial class IngredientDto
    {
        [JsonProperty("IngredientId")]
        public string IngredientId { get; set; }

        [JsonProperty("IngredientName")]
        public string IngredientName { get; set; }
    }
}