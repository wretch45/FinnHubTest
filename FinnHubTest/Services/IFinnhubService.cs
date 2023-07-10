using System.Threading.Tasks;
using FinnHubTest.Models;
using System.Collections.Generic;

namespace FinnHubTest.Services;

public interface IFinnhubService
{
    Task<List<ResultItem>> SearchSymbol(string query);
    Task<Stock> GetStockInformation(string symbol);
    Task<string> GetStockName(string symbol);
}
