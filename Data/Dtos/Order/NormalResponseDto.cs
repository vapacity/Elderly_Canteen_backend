namespace Elderly_Canteen.Data.Dtos.Order
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class NormalResponseDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}