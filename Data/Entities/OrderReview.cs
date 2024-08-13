using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class OrderReview
{
    public string OrderId { get; set; } = null!;

    public decimal? CStars { get; set; }

    public string? CReviewText { get; set; }

    public virtual OrderInf Order { get; set; } = null!;
}
