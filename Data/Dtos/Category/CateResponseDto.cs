namespace Elderly_Canteen.Data.Dtos.Category
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Request
    /// </summary>
    public partial class CateResponseDto
    {
        [JsonProperty("cates")]
        public CateDto Cates { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public partial class CateDto
    {
        [JsonProperty("cateId")]
        public string CateId { get; set; }

        [JsonProperty("cateName")]
        public string CateName { get; set; }
    }
}