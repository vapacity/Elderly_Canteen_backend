using System;
using System.Collections.Generic;

namespace Elderly_Canteen.Data.Entities;

public partial class Administrator
{
    public string AccountId { get; set; } = null!;

    public string AccountName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string PhoneNum { get; set; } = null!;

    public int? VerifyCode { get; set; }

    public string Identity { get; set; } = null!;

    public byte[] Portrait { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public DateTime? BirthDate { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public string? IdCard { get; set; }

    public string? Email { get; set; }

    public string Position { get; set; } = null!;

    public virtual ICollection<Finance> Finances { get; set; } = new List<Finance>();

    public virtual ICollection<PayWage> PayWages { get; set; } = new List<PayWage>();

    public virtual ICollection<Restock> Restocks { get; set; } = new List<Restock>();

    public virtual ICollection<VolReview> VolReviews { get; set; } = new List<VolReview>();
}
