using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Cart
{
    public string CartId { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public DateTime CreatedTime { get; set; }

    public DateTime UpdatedTime { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<DeliverOrder> DeliverOrders { get; set; } = new List<DeliverOrder>();

    public virtual ICollection<OrderInf> OrderInfs { get; set; } = new List<OrderInf>();
}
