namespace Elderly_Canteen.Data.Dtos.Finance
{
    public class FinanceTotalsResponse
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public FinanceData response { get; set; }
    }

    public class FinanceData
    {
        public decimal NetIn { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
    }
}
