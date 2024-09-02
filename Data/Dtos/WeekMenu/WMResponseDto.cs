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
    public partial class WmResponseDto
    {
        [JsonProperty("dish")]
        public DishInfo Dish { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public partial class DishInfo
    {
        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("dishName")]
        public string DishName { get; set; }
    }
}