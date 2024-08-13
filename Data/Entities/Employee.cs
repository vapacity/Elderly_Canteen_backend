using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Employee
{
    public string EmployeeId { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string PhoneNum { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string EmployeePosition { get; set; } = null!;

    public decimal Salary { get; set; }

    public string IdCard { get; set; } = null!;

    public virtual ICollection<DeliverE> DeliverEs { get; set; } = new List<DeliverE>();

    public virtual ICollection<PayWage> PayWages { get; set; } = new List<PayWage>();
}
