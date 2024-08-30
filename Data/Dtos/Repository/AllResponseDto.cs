namespace Elderly_Canteen.Data.Dtos.Repository
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Request
    /// </summary>
    public partial class AllResponseDto
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
        [JsonProperty("account")]
        public double Account { get; set; }

        [JsonProperty("expiry")]
        public DateTime Expiry { get; set; }

        [JsonProperty("grade")]
        public byte Grade { get; set; }

        [JsonProperty("IngredientId")]
        public string IngredientId { get; set; }

        [JsonProperty("IngredientName")]
        public string IngredientName { get; set; }
    }
}