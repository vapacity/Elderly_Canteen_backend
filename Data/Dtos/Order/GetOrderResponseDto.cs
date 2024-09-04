namespace Elderly_Canteen.Data.Dtos.Order
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class GetOrderResponseDto
    {
        /// <summary>
        /// 提示
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }

        /// <summary>
        /// 响应
        /// </summary>
        [JsonProperty("response")]
        public OrderItem[] Response { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
    }

}