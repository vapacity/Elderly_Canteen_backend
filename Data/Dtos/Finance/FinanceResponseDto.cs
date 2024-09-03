namespace Elderly_Canteen.Data.Dtos.Finance
{
    public class FinanceResponseDto
    {
        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 员工信息列表
        /// </summary>
        public List<FinanceResponseData> response { get; set; }
    }

    public class FinanceResponseData
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
    }
}
