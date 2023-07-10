using System.Text.Json.Serialization;

namespace FinnHubTest.Models
{
    public class SearchResult
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("result")]
        public List<ResultItem> Result { get; set; } = new List<ResultItem>();
    }

    public class ResultItem
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}