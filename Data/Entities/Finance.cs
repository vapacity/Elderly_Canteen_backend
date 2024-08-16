using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Finance
{
    public string FinanceId { get; set; } = null!;

    public string FinanceType { get; set; } = null!;

    public DateTime FinanceDate { get; set; }

    public decimal Price { get; set; }

    public string InOrOut { get; set; } = null!;

    public string AccountId { get; set; } = null!;

    public string? AdministratorId { get; set; }

    public byte[]? Proof { get; set; }

    public string Status { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual Administrator? Administrator { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual ICollection<OrderInf> OrderInfs { get; set; } = new List<OrderInf>();

    public virtual ICollection<PayWage> PayWages { get; set; } = new List<PayWage>();

    public virtual ICollection<Restock> Restocks { get; set; } = new List<Restock>();
}
