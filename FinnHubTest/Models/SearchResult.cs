using System.Text.Json.Serialization;

namespace FinnHubTest.Models;
public class SearchResult
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("result")]
    public List<ResultItem> Result { get; set; }
}

public class ResultItem
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("displaySymbol")]
    public string DisplaySymbol { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}