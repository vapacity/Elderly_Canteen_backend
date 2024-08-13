using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Volunteer
{
    public string AccountId { get; set; } = null!;

    public string Available { get; set; } = null!;

    public decimal? Score { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<DeliverEmployee> DeliverEmployees { get; set; } = new List<DeliverEmployee>();

    public virtual ICollection<DeliverV> DeliverVs { get; set; } = new List<DeliverV>();
}
