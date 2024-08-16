namespace Elderly_Canteen.Data.Dtos.EmployeeInfo
{
    public class EmployeeInfoResponseDto
    {
        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 员工信息列表
        /// </summary>
        public List<EmployeeResponseData> Response { get; set; }
    }

    public class EmployeeResponseData
    {
        /// <summary>
        /// 员工ID
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNum { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

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
