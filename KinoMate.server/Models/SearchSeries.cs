using System.Text.Json.Serialization;

namespace KinoMate.server.Models
{
    public class SearchSeries
    {
        public class SearchSeriesResponse
        {
            public List<SearchSeriesResult> Results { get; set; }
        }

        public class SearchSeriesResult
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("type")]
            public string Type { get; set; }
        }
        public class SearchSeriesResponseMapping
        {
            [JsonPropertyName("page")]
            public int Page { get; set; }
            [JsonPropertyName("results")]
            public List<SearchSeriesResult> Results { get; set; }
            [JsonPropertyName("total_results")]
            public int TotalResults { get; set; }
            [JsonPropertyName("total_pages")]
            public int TotalPages { get; set; }
        }
    }
}
