namespace FinnHubTest.Models;

public class Stock
{
    public string Symbol { get; set; }
    public decimal Price { get; set; }
    public long Timestamp { get; set; }
    public decimal PercentChange { get; set; }
}

