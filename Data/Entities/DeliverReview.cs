using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class DeliverReview
{
    public string OrderId { get; set; } = null!;

    public decimal? DStars { get; set; }

    public string? DReviewText { get; set; }

    public virtual OrderInf Order { get; set; } = null!;
}
