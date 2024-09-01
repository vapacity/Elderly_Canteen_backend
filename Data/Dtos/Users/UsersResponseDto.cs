namespace Elderly_Canteen.Data.Dtos.Users
{
    public class UsersResponseDto
    {
        public bool Success { get; set; }

        public string Msg { get; set; }

        public List<UsersResponseData> Response { get; set; }
    }

    public class UsersResponseData
    {

        public string Name { get; set; }

        public string IdCard { get; set; }

        public string BirthDate { get; set; }

        public string Address { get; set; }
    }

}