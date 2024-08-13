using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class DeliverEmployee
{
    public string EmployeeId { get; set; } = null!;

    public string? VolunteerId { get; set; }

    public virtual Volunteer? Volunteer { get; set; }
}
