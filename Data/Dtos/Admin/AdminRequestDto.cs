namespace Elderly_Canteen.Data.Dtos.Admin
{
    public class AdminRequestDto
    {
        public string? AccountId { get; set; }

        public string? Name { get; set; }

        public string PhoneNum { get; set; }

        public string Position { get; set; }

        public string Gender { get; set; }
    }

    public class AdminRegisterDto
    {
        public string Name { get; set; }

        public string PhoneNum { get; set; }

        public string Position { get; set; }

        public string Gender { get; set; }

        public string IdCard { get; set; }

        public string BirthDate { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }
    }

    public class AdminInfoChangeDto
    {
        public string? PhoneNum { get; set; }

        public string? Gender { get; set; }

        public string? BirthDate { get; set; }

        public string? AccountName { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }
    }
}
