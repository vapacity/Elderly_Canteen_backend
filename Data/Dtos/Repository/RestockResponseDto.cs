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
    public partial class RestockResponseDto
    {
        [JsonProperty("data")]
        public RestockData Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

    }

    public partial class RestockData
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("expiry")]
        public DateTime Expiry { get; set; }

        [JsonProperty("financeId")]
        public string FinanceId { get; set; }

        [JsonProperty("grade")]
        public int Grade { get; set; }

        [JsonProperty("ingredientId")]
        public string IngredientId { get; set; }

        [JsonProperty("ingredientName")]
        public string IngredientName { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}