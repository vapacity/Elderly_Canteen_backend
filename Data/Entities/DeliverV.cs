using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class DeliverV
{
    public string OrderId { get; set; } = null!;

    public string VolunteerId { get; set; } = null!;

    public virtual OrderInf Order { get; set; } = null!;

    public virtual Volunteer Volunteer { get; set; } = null!;
}
