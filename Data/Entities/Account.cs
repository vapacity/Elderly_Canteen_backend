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

    public string? Portrait { get; set; }

    public string? Gender { get; set; }

    public DateTime? Birthdate { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public string? Idcard { get; set; }
}
