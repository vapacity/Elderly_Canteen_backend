namespace Elderly_Canteen.Data.Dtos.Order
{
    public class ReviewSubmissionDto
    {
        public string OrderId { get; set; }
        public int CStars { get; set; }
        public string CReviewText { get; set; }

        public int? DStars { get; set; }
        public string? DReviewText { get; set; }
    }
}
