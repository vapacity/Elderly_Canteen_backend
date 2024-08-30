using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Account
{
    public string Accountid { get; set; } = null!;

    public string Accountname { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Phonenum { get; set; } = null!;

    public int? Verifycode { get; set; }

    public string Identity { get; set; } = null!;

    public string? Gender { get; set; }

    public DateTime? Birthdate { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public string? Idcard { get; set; }

    public string? Portrait { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual ICollection<Finance> Finances { get; set; } = new List<Finance>();

    public virtual Senior? Senior { get; set; }

    public virtual ICollection<VolApplication> VolApplications { get; set; } = new List<VolApplication>();

    public virtual Volunteer? Volunteer { get; set; }
}
