namespace Elderly_Canteen.Data.Dtos.OTP
{
    public class VerifyOTPRequestDto
    {
        public string PhoneNum { get; set; }
        public string Code { get; set; }
        public string? NewPassword { get; set; }

        public string? NewPhoneNum { get; set; }
    }
}
