using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Donation
{
    public string DonationId { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public string FinanceId { get; set; } = null!;

    public string Origin { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual Finance Finance { get; set; } = null!;
}
