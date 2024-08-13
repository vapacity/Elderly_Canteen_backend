using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class PayWage
{
    public string FinanceId { get; set; } = null!;

    public string EmployeeId { get; set; } = null!;

    public string AdministratorId { get; set; } = null!;

    public virtual Administrator Administrator { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Finance Finance { get; set; } = null!;
}
