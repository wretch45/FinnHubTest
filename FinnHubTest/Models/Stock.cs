namespace FinnHubTest.Models;

public class Stock
{
    public string Symbol { get; set; }
    public decimal CurrentPrice { get; set; }  // renamed from 'Price' to 'CurrentPrice'
    public decimal Change { get; set; }  // new field
    public decimal PercentChange { get; set; }  // existing field, now it aligns with API response 'dp'
    public decimal HighPriceOfDay { get; set; }  // new field
    public decimal LowPriceOfDay { get; set; }  // new field
    public decimal OpenPriceOfDay { get; set; }  // new field
    public decimal PreviousClosePrice { get; set; }  // new field
    public long Timestamp { get; set; }
}