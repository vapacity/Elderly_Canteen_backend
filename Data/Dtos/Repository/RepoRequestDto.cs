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
    public partial class RepoRequestDto
    {
        /// <summary>
        /// 数量
        /// </summary>
        [JsonProperty("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [JsonProperty("grade")]
        public byte Grade { get; set; }

        /// <summary>
        /// ID 编号
        /// </summary>
        [JsonProperty("ingredientId")]
        public string IngredientId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty("ingredientName")]
        public string IngredientName { get; set; }


        /// <summary>
        /// 新保质期用于插入和修改
        /// </summary>
        [JsonProperty("newExpiry")]
        public DateTime NewExpiry { get; set; }

        /// <summary>
        /// 老保质期
        /// </summary>
        [JsonProperty("oldExpiry", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? OldExpiry { get; set; }
    }
}