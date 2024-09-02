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
    public partial class CateRequestDto
    {
        [JsonProperty("cateId")]
        public string? CateId { get; set; }

        [JsonProperty("cateName")]
        public string CateName { get; set; }
    }
}