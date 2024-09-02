using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Weekmenu
{
    public string DishId { get; set; } = null!;

    public DateTime Week { get; set; }

    public int Stock { get; set; }

    public int Sales { get; set; }

    public decimal DisPrice { get; set; }

    public string Day { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
