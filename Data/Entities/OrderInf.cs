using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class OrderInf
{
    public string OrderId { get; set; } = null!;

    public string DeliverOrDining { get; set; } = null!;

    public string FinanceId { get; set; } = null!;

    public string CartId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal Bonus { get; set; }

    public string? Remark { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual DeliverE? DeliverE { get; set; }

    public virtual DeliverOrder? DeliverOrder { get; set; }

    public virtual DeliverReview? DeliverReview { get; set; }

    public virtual DeliverV? DeliverV { get; set; }

    public virtual Finance Finance { get; set; } = null!;

    public virtual OrderReview? OrderReview { get; set; }
}
