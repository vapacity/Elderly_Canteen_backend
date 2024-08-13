using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class VwDeliverEmployeeDetail
{
    public string EmployeeId { get; set; } = null!;

    public string? VolunteerId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string PhoneNum { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string EmployeePosition { get; set; } = null!;

    public decimal Salary { get; set; }

    public string IdCard { get; set; } = null!;
}
