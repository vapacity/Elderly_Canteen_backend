using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Dish
{
    public string DishId { get; set; } = null!;

    public byte[] Picture { get; set; } = null!;

    public string DishName { get; set; } = null!;

    public decimal Price { get; set; }

    public string CateId { get; set; } = null!;

    public virtual Category Cate { get; set; } = null!;

    public virtual ICollection<Formula> Formulas { get; set; } = new List<Formula>();
}
