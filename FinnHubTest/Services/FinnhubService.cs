using System.Text.Json;
using System.Threading.Tasks;
using FinnHubTest.Models;

namespace FinnHubTest.Services
{
    public class FinnhubService : IFinnhubService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://finnhub.io/api/v1/";
        private const string ApiKey = "cie7kvhr01qmfas4a580cie7kvhr01qmfas4a58g";

        public FinnhubService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultItem>> SearchSymbol(string query)
        {
            var url = $"{BaseUrl}search?q={query}&token={ApiKey}";

            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Handle API request failure
                throw new Exception($"Failed to retrieve stock information. Status code: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize the response JSON
            var searchResult = JsonSerializer.Deserialize<SearchResult>(responseContent);

            if (searchResult != null && searchResult.Count > 0)
            {
                // Filter the results to only include items of type "Common Stock"
                var commonStockResults = searchResult.Result.Where(r => r.Type == "Common Stock").ToList();

                // Order the result by the length of the symbol in ascending order
                return commonStockResults.OrderBy(r => r.Symbol.Length).ToList();
            }

            // If no results are found, return an empty list
            return new List<ResultItem>();
        }
        
        public async Task<Stock> GetStockInformation(string symbol)
        {
            var url = $"{BaseUrl}quote?symbol={symbol}&token={ApiKey}";

            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Handle API request failure
                throw new Exception($"Failed to retrieve stock information. Status code: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize the response JSON
            var stockData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

            if(stockData == null)
            {
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

        private decimal TryParseToDecimal(object value)
        {
            if(value != null)
            {
                decimal.TryParse(value.ToString(), out var result);
                return result;
            }
            return 0;
        }

        private long TryParseToLong(object value)
        {
            if(value != null)
            {
                long.TryParse(value.ToString(), out var result);
                return result;
            }
            return 0;
        }

        
        public async Task<string> GetStockName(string symbol)
        {
            var url = $"{BaseUrl}search?q={symbol}&token={ApiKey}";

            Console.WriteLine($"URL: {url}"); // Log the URL
            Console.WriteLine($"Symbol: {symbol}"); // Log the Symbol

            var response = await _httpClient.GetAsync(url);

            Console.WriteLine($"Response: {response}"); // Log the response

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response Content: {responseContent}"); // Log the response content

                var searchResult = JsonSerializer.Deserialize<SearchResult>(responseContent);

                Console.WriteLine($"Search Result: {searchResult}"); // Log the search result

                if (searchResult != null && searchResult.Count > 0)
                {
                    Console.WriteLine("Search result is not null and has more than 0 items."); // Log progress

                    // Search the result collection for the correct stock
                    foreach (var result in searchResult.Result)
                    {
                        Console.WriteLine($"Checking result: {result}"); // Log current result

                        if (result.Symbol == symbol)
                        {
                            Console.WriteLine("Found matching stock."); // Log success
                            return result.Description;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Search result is either null or empty."); // Log issue
                }
            }
            else
            {
                Console.WriteLine($"Failed to retrieve data from API. Status code: {response.StatusCode}"); // Log API failure
            }

            Console.WriteLine("No matching stock found."); // Log failure
            return "Unknown Stock"; // Return 'Unknown Stock' if no matching stock is found
        }
    }
}
