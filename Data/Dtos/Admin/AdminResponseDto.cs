namespace Elderly_Canteen.Data.Dtos.Admin
{
    public class AdminResponseDto
    {
        public bool Success { get; set; }

        public string Msg { get; set; }

        public List<AdminResponseData> Response { get; set; }
    }

    public class AdminResponseData
    {

        public string Name { get; set; }

        public string IdCard { get; set; }

        public string BirthDate { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }
    }

    public class AdminSearchDto
    {
        public bool Success { get; set; }

        public string Msg { get; set; }

        public List<AdminSearchData> Response { get; set; }
    }

    public class AdminSearchData
    {

        public string name { get; set; }

        public string accountId { get; set; }

        public string phoneNum { get; set; }

        public string position { get; set; }

        public string gender { get; set; }
    }
}
