using Elderly_Canteen.Data.Dtos.EmployeeInfo;

namespace Elderly_Canteen.Data.Dtos.Order
{
    public class ReviewResponseDto
    {/// <summary>
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
        public List<ReviewResponseData> response { get; set; }
    }
    public class ReviewResponseData
    {
        public decimal CStars { get; set; }
        public string CReviewText { get; set; }
        public decimal? DStars { get; set; }
        public string? DReviewText { get; set; }
    }
}
