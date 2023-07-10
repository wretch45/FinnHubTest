namespace FinnHubTest.Models
{
    public class Stock
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }  
        public decimal Change { get; set; } 
        public decimal PercentChange { get; set; } 
        public decimal HighPriceOfDay { get; set; } 
        public decimal LowPriceOfDay { get; set; } 
        public decimal OpenPriceOfDay { get; set; }
        public decimal PreviousClosePrice { get; set; }
        public long Timestamp { get; set; }
    }
}