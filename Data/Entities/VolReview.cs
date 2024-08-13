using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class VolReview
{
    public string ApplicationId { get; set; } = null!;

    public string AdministratorId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public DateTime ReviewDate { get; set; }

    public virtual Administrator Administrator { get; set; } = null!;

    public virtual VolApplication Application { get; set; } = null!;
}
