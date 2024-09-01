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
    public partial class AllRestockResponseDto
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("restocks")]
        public List<Restocks> Restocks { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public partial class Restocks
    {
        [JsonProperty("administratorId")]
        public string AdministratorId { get; set; }

        [JsonProperty("administratorName")]
        public string AdministratorName { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("expirationTime")]
        public DateTime ExpirationTime { get; set; }

        [JsonProperty("financeId")]
        public string FinanceId { get; set; }

        [JsonProperty("ingredientId")]
        public string IngredientId { get; set; }

        [JsonProperty("ingredientName")]
        public string IngredientName { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}