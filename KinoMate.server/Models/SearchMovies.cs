using System.Text.Json.Serialization;

namespace KinoMate.server.Models
{
    public class SearchResponse
    {
        public List<SearchResult> Results { get; set; }
    }

    public class SearchResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
    public class SearchMoviesResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }
        [JsonPropertyName("results")]
        public List<SearchResult> Results { get; set; }
        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
    }
}
