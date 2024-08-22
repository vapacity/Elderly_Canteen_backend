namespace Elderly_Canteen.Data.Dtos.OTP
{
    public class VerifyOTPResponseDto<T>
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public T? Response { get; set; }
    }
    public class OTPLoginResponseDto
    {
        public string? Token { get; set; }
        public string? Identity { get; set; }
        public string? AccountName { get; set; }
        public string? AccountId { get; set; }
    }
}
