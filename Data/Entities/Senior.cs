using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Senior
{
    public string AccountId { get; set; } = null!;

    public string FamilyNum { get; set; } = null!;

    public decimal Subsidy { get; set; }

    public virtual Account Account { get; set; } = null!;
}
