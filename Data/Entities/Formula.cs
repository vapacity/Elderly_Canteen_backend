using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Formula
{
    public string DishId { get; set; } = null!;

    public string IngredientId { get; set; } = null!;

    public short Amount { get; set; }

    public virtual Dish Dish { get; set; } = null!;

    public virtual Ingredient Ingredient { get; set; } = null!;
}
