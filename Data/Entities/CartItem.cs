using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class CartItem
{
    public string CartId { get; set; } = null!;

    public string DishId { get; set; } = null!;

    public DateTime Week { get; set; }

    public int Quantity { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Weekmenu Weekmenu { get; set; } = null!;
}
