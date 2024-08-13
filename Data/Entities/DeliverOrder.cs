using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class DeliverOrder
{
    public string OrderId { get; set; } = null!;

    public string DeliverPhone { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string CusAddress { get; set; } = null!;

    public string DeliverStatus { get; set; } = null!;

    public string? CartId { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual OrderInf Order { get; set; } = null!;
}
