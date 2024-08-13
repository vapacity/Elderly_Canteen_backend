using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Category
{
    public string CateId { get; set; } = null!;

    public string CateName { get; set; } = null!;

    public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
