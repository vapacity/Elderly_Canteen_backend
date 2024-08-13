using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class VwDeliverOrderDetail
{
    public string OrderId { get; set; } = null!;

    public string DeliverOrDining { get; set; } = null!;

    public string FinanceId { get; set; } = null!;

    public string CartId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal Bonus { get; set; }

    public string? Remark { get; set; }

    public string DeliverPhone { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string CusAddress { get; set; } = null!;

    public string DeliverStatus { get; set; } = null!;
}
