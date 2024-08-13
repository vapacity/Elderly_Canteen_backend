namespace Elderly_Canteen.Data.Dtos.Dish
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class DishResponseDto
    {
        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("response")]
        public List<Response> Response { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("price", NullValueHandling = NullValueHandling.Ignore)]
        public string Price { get; set; }

        [JsonProperty("recipe", NullValueHandling = NullValueHandling.Ignore)]
        public string Recipe { get; set; }
    }
}