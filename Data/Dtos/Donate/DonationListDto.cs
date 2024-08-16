namespace Elderly_Canteen.Data.Dtos.Donate
{
    public class DonationListDto
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public List<ResponceData> responce { get; set; } // 改为List类型

        public class ResponceData
        {
            public string origin { get; set; }
            public double price { get; set; }
            public string financeDate { get; set; }
        }
    }
}
