using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FinnHubTest.Models;
using Microsoft.Extensions.Logging;

namespace FinnHubTest.Services
{
    public class FinnhubService : IFinnhubService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://finnhub.io/api/v1/";
        private const string ApiKey = "cie7kvhr01qmfas4a580cie7kvhr01qmfas4a58g";
        private readonly ILogger<FinnhubService> _logger;

        // Constructor for the FinnhubService class
        public FinnhubService(HttpClient httpClient, ILogger<FinnhubService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // Method to search for a stock symbol
        public async Task<List<ResultItem>> SearchSymbol(string query)
        {
            try
            {
                var url = $"{BaseUrl}search?q={query}&token={ApiKey}";

                using var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to retrieve stock information. Status code: {response.StatusCode}");
                    throw new Exception($"Failed to retrieve stock information. Status code: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var searchResult = JsonSerializer.Deserialize<SearchResult>(responseContent);

                if (searchResult != null && searchResult.Count > 0)
                {
                    var commonStockResults = searchResult.Result.Where(r => r.Type == "Common Stock").ToList();
                    return commonStockResults.OrderBy(r => r.Symbol.Length).ToList();
                }

                return new List<ResultItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for the symbol.");
                throw;
            }
        }

        // Method to get information about a stock
        public async Task<Stock> GetStockInformation(string symbol)
        {
            try
            {
                var url = $"{BaseUrl}quote?symbol={symbol}&token={ApiKey}";

                using var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to retrieve stock information. Status code: {response.StatusCode}");
                    throw new Exception($"Failed to retrieve stock information. Status code: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var stockData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

                if(stockData == null)
                {
                    _logger.LogError("Unable to parse stock data.");
                    throw new Exception("Unable to parse stock data.");
                }

                var stock = new Stock
                {
                    Symbol = symbol,
                    CurrentPrice = TryParseToDecimal(stockData["c"]),
                    Change = TryParseToDecimal(stockData["d"]),
                    PercentChange = TryParseToDecimal(stockData["dp"]),
                    HighPriceOfDay = TryParseToDecimal(stockData["h"]),
                    LowPriceOfDay = TryParseToDecimal(stockData["l"]),
                    OpenPriceOfDay = TryParseToDecimal(stockData["o"]),
                    PreviousClosePrice = TryParseToDecimal(stockData["pc"]),
                    Timestamp = TryParseToLong(stockData["t"]) * 1000  // Convert from seconds to milliseconds
                };

                return stock;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving stock information.");
                throw;
            }
        }

        // Helper method to parse a value to decimal
        private decimal TryParseToDecimal(object value)
        {
            if(value != null)
            {
                decimal.TryParse(value.ToString(), out var result);
                return result;
            }
            return 0;
        }

        // Helper method to parse a value to long
        private long TryParseToLong(object value)
        {
            if(value != null)
            {
                long.TryParse(value.ToString(), out var result);
                return result;
            }
            return 0;
        }

        // Method to get the name of a stock
        public async Task<string> GetStockName(string symbol)
        {
            try
            {
                var url = $"{BaseUrl}search?q={symbol}&token={ApiKey}";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var searchResult = JsonSerializer.Deserialize<SearchResult>(responseContent);

                    if (searchResult != null && searchResult.Count > 0)
                    {
                        foreach (var result in searchResult.Result)
                        {
                            if (result.Symbol == symbol)
                            {
                                return result.Description;
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogError($"Failed to retrieve data from API. Status code: {response.StatusCode}");
                    throw new Exception($"Failed to retrieve data from API. Status code: {response.StatusCode}");
                }

                return "Unknown Stock";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the stock name.");
                throw;
            }
        }
    }
}
