namespace Elderly_Canteen.Data.Dtos.WeekMenu
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Request
    /// </summary>
    public partial class AllWeekMenuResponseDto
    {
        [JsonProperty("Fri")]
        public List<Fri> Fri { get; set; }

        [JsonProperty("Mon")]
        public List<Mon> Mon { get; set; }

        [JsonProperty("Sat")]
        public List<Sat> Sat { get; set; }

        [JsonProperty("Sun")]
        public List<Sun> Sun { get; set; }

        [JsonProperty("Thu")]
        public List<Thu> Thu { get; set; }

        [JsonProperty("Tue")]
        public List<Tue> Tue { get; set; }

        [JsonProperty("Wed")]
        public List<Wed> Wed { get; set; }
    }

    public partial class Fri
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("originalPrice")]
        public decimal OriginalPrice { get; set; }

        [JsonProperty("discountPrice")]
        public decimal DiscountPrice { get; set; }
    }

    public partial class Mon
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("originalPrice")]
        public decimal OriginalPrice { get; set; }

        [JsonProperty("discountPrice")]
        public decimal DiscountPrice { get; set; }
    }

    public partial class Sat
    {
        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("originalPrice")]
        public decimal OriginalPrice { get; set; }

        [JsonProperty("discountPrice")]
        public decimal DiscountPrice { get; set; }
    }

    public partial class Sun
    {
        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("originalPrice")]
        public decimal OriginalPrice { get; set; }

        [JsonProperty("discountPrice")]
        public decimal DiscountPrice { get; set; }
    }

    public partial class Thu
    {
        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("originalPrice")]
        public decimal OriginalPrice { get; set; }

        [JsonProperty("discountPrice")]
        public decimal DiscountPrice { get; set; }
    }

    public partial class Tue
    {
        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty("originalPrice")]
        public decimal OriginalPrice { get; set; }

        [JsonProperty("discountPrice")]
        public decimal DiscountPrice { get; set; }
    }

    public partial class Wed
    {
        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("originalPrice")]
        public decimal OriginalPrice { get; set; }

        [JsonProperty("discountPrice")]
        public decimal DiscountPrice { get; set; }
    }
}