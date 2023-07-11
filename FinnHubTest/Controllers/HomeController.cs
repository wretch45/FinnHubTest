using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinnHubTest.Models;
using FinnHubTest.Services;
using Microsoft.Extensions.Logging;

namespace FinnHubTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFinnhubService _finnhubService;
        private readonly ILogger<HomeController> _logger;

        // Constructor for the HomeController class
        public HomeController(IFinnhubService finnhubService, ILogger<HomeController> logger)
        {
            _finnhubService = finnhubService;
            _logger = logger;
        }

        // Method to handle the Index action
        public IActionResult Index()
        {
            _logger.LogInformation("Index page visited.");
            return View();
        }

        // Method to handle the Error action
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet] // Method to handle the SearchSymbol action
        public async Task<IActionResult> SearchSymbol(string query)
        {
            try
            {
                var searchResultItems = await _finnhubService.SearchSymbol(query);

                if (searchResultItems.Count == 0)
                {
                    _logger.LogError($"No matching symbols found for query: {query}.");
                    return Json(new { error = "No matching symbols found." });
                }

                return Json(searchResultItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while searching for symbol: {query}.");
                return Json(new { error = "An error occurred while searching for symbol." });
            }
        }

        // Method to handle the GetStockInformation action
        public async Task<IActionResult> GetStockInformation(string symbol)
        {
            try
            {
                var stock = await _finnhubService.GetStockInformation(symbol);

                if (stock == null || stock.CurrentPrice == 0)
                {
                    _logger.LogError($"Invalid ticker symbol or stock information not available for symbol: {symbol}.");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting stock information for symbol: {symbol}.");
                return Json(new { error = "An error occurred while getting stock information." });
            }
        }

        // Method to handle the GetStockName action
        public async Task<IActionResult> GetStockName(string symbol)
        {
            try
            {
                var name = await _finnhubService.GetStockName(symbol);

                if (string.IsNullOrEmpty(name))
                {
                    _logger.LogError($"No matching stock name found for symbol: {symbol}.");
                    return Json(new { name = "Unknown Stock" });
                }

                return Json(new { name = name });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting stock name for symbol: {symbol}.");
                return Json(new { error = "An error occurred while getting stock name." });
            }
        }
    }
}
