namespace Elderly_Canteen.Data.Dtos.EmployeeInfo
{
    public class EmployeeInfoRequestDto
    {
        /// <summary>
        /// 员工ID
        /// </summary>
        public string? EmployeeId { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 员工电话
        /// </summary>
        public string PhoneNum { get; set; }

        /// <summary>
        /// 员工地址
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string EmployeePosition { get; set; }

        /// <summary>
        /// 薪水
        /// </summary>
        public decimal Salary { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 是否支付薪水
        /// </summary>
        public bool? IsPaidThisMonth { get; set; }
    }
}