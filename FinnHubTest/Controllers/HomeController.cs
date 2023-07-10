using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinnHubTest.Models;
using FinnHubTest.Services;

namespace FinnHubTest.Controllers;

public class HomeController : Controller
{
    private readonly IFinnhubService _finnhubService;

    public HomeController(IFinnhubService finnhubService)
    {
        _finnhubService = finnhubService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public async Task<IActionResult> SearchSymbol(string query)
    {
        var searchResultItems = await _finnhubService.SearchSymbol(query);

        if (searchResultItems.Count == 0)
        {
            return Json(new { error = "No matching symbols found." });
        }

        return Json(searchResultItems);
    }

    public async Task<IActionResult> GetStockInformation(string symbol)
    {
        var stock = await _finnhubService.GetStockInformation(symbol);

        if (stock == null || stock.CurrentPrice == 0)
        {
            return Json(new { error = "Invalid ticker symbol or stock information not available." });
        }

        return Json(new
        {
            timestamp = DateTimeOffset.FromUnixTimeMilliseconds(stock.Timestamp).ToLocalTime()
                .ToString("yyyy-MM-dd HH:mm:ss"),
            symbol = stock.Symbol,
            currentPrice = stock.CurrentPrice,
            change = stock.Change,
            percentChange = stock.PercentChange,
            highPriceOfDay = stock.HighPriceOfDay,
            lowPriceOfDay = stock.LowPriceOfDay,
            openPriceOfDay = stock.OpenPriceOfDay,
            previousClosePrice = stock.PreviousClosePrice
        });
    }


    public async Task<IActionResult> GetStockName(string symbol)
    {
        var name = await _finnhubService.GetStockName(symbol);

        if (string.IsNullOrEmpty(name))
        {
            return Json(new { name = "Unknown Stock" });
        }

        return Json(new { name = name });
    }
}
