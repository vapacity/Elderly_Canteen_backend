using Newtonsoft.Json;

namespace Elderly_Canteen.Data.Dtos.Ingredient
{
    public class IngreResponseDto
    {
        public bool success { get; set; }
        public string message { get; set; }

        [JsonProperty("IngredientId")]
        public string? IngredientId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty("IngredientName")]
        public string? IngredientName { get; set; }
    }
}
