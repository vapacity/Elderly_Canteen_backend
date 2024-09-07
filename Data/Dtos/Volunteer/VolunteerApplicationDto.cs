namespace Elderly_Canteen.Data.Dtos.Volunteer
{
    public class VolunteerApplicationDto
    {
        public List<VolunteerApplyDto> Vols { get; set; } = new List<VolunteerApplyDto>();
    }


        public class VolunteerApplyDto
        {
            public string accountId { set; get; }
            public string? selfStatement { set; get; }
        }
    
    public class VolunteerReviewApplicationDto
    {
        public string reason { get; set; }
        public string status { get; set; }
    }
}
