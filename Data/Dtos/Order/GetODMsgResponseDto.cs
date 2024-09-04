namespace Elderly_Canteen.Data.Dtos.Order
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class GetOdMsgResponseDto
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
        public VolunteerMsg Response { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    /// <summary>
    /// 响应
    /// </summary>
    public partial class VolunteerMsg
    {
        /// <summary>
        /// 志愿者Id
        /// </summary>
        [JsonProperty("VolunteerId")]
        public string VolunteerId { get; set; }

        /// <summary>
        /// 志愿者姓名
        /// </summary>
        [JsonProperty("VolunteerName")]
        public string VolunteerName { get; set; }
    }
}