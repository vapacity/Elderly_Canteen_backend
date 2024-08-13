using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class DeliverE
{
    public string OrderId { get; set; } = null!;

    public string EmployeeId { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual OrderInf Order { get; set; } = null!;
}
