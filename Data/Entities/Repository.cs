using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Repository
{
    public string IngredientId { get; set; } = null!;

    public string IngredientName { get; set; } = null!;

    public int RemainAmount { get; set; }

    public bool HighConsumption { get; set; }

    public DateTime ExpirationTime { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual Ingredient IngredientNameNavigation { get; set; } = null!;
}
