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
    public partial class IngreRequestDto
    {
        /// <summary>
        /// 数量
        /// </summary>
        [JsonProperty("account")]
        public int Account { get; set; }

        /// <summary>
        /// 保质期（月）
        /// </summary>
        [JsonProperty("newExpiry")]
        public DateTime newExpiry { get; set; }

        [JsonProperty("oldExpiry")]
        public DateTime? oldExpiry { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [JsonProperty("grade")]
        public byte Grade { get; set; }

        [JsonProperty("IngredientId")]
        public string? IngredientId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty("IngredientName")]
        public string? IngredientName { get; set; }
    }
}