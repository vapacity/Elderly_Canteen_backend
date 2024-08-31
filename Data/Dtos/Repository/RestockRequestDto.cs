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
    public partial class RestockRequestDto
    {
        /// <summary>
        /// 进货数量
        /// </summary>
        [JsonProperty("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [JsonProperty("expiry")]
        public DateTime Expiry { get; set; }

        /// <summary>
        /// ID 编号
        /// </summary>
        [JsonProperty("ingredientId")]
        public string IngredientId { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}