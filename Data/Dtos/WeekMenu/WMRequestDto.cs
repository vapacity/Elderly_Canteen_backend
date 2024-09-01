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
    public partial class WmRequestDto
    {
        /// <summary>
        /// 当天日期
        /// </summary>
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// ID 编号
        /// </summary>
        [JsonProperty("dishId")]
        public string DishId { get; set; }
    }
}