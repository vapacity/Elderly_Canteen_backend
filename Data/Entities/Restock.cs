using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Restock
{
    public string FinanceId { get; set; } = null!;

    public string IngredientId { get; set; } = null!;

    public string AdministratorId { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Administrator Administrator { get; set; } = null!;

    public virtual Finance Finance { get; set; } = null!;

    public virtual Ingredient Ingredient { get; set; } = null!;
}
