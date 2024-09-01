using Newtonsoft.Json;

namespace Elderly_Canteen.Data.Dtos.Dish
{
    public class AllDishResponseDto
    {
        [JsonProperty("dish")]
        public List<DishDto> Dish { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
