using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Administrator
{
    public string AccountId { get; set; } = null!;

    public string? Email { get; set; }

    public string Position { get; set; } = null!;

    public virtual ICollection<Finance> Finances { get; set; } = new List<Finance>();

    public virtual ICollection<PayWage> PayWages { get; set; } = new List<PayWage>();

    public virtual ICollection<Restock> Restocks { get; set; } = new List<Restock>();

    public virtual ICollection<VolReview> VolReviews { get; set; } = new List<VolReview>();
}
