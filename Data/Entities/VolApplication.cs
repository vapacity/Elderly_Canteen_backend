using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class VolApplication
{
    public string ApplicationId { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public string SelfStatement { get; set; } = null!;

    public DateTime ApplicationDate { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual VolReview? VolReview { get; set; }
}
