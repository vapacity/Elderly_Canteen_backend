namespace Elderly_Canteen.Data.Dtos.VolServe
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class AcceptRequestDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [JsonProperty("ORDER_ID")]
        public string OrderId { get; set; }
    }
}