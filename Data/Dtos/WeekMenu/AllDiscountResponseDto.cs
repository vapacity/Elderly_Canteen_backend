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
    public partial class AllDiscountResponseDto
    {
        [JsonProperty("dishes")]
        public List<DishDto> Dishes { get; set; }
    }

    public partial class DishDto
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("currentPrice")]
        public decimal CurrentPrice { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("originalPrice")]
        public decimal OriginalPrice { get; set; }
    }
}