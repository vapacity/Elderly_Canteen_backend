using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Ingredient
{
    public string IngredientId { get; set; } = null!;

    public string IngredientName { get; set; } = null!;

    public virtual ICollection<Formula> Formulas { get; set; } = new List<Formula>();

    public virtual ICollection<Repository> Repositories { get; set; } = new List<Repository>();

    public virtual ICollection<Restock> Restocks { get; set; } = new List<Restock>();
}
