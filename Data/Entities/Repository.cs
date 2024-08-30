using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Repository
{
    public string IngredientId { get; set; } = null!;

    public int RemainAmount { get; set; }

    public byte HighConsumption { get; set; }

    public DateTime ExpirationTime { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;
}
