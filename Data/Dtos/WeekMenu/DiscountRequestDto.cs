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
    public partial class DiscountRequestDto
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        [JsonProperty("dishId")]
        public string DishId { get; set; }
    }
}