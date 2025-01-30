using System.Text.Json.Serialization;

namespace KinoMate.server.Models
{
    public class SeriesShort
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("popularity")]
        public double? Popularity { get; set; }

        [JsonPropertyName("first_air_date")]
        public string? FirstAirDate { get; set; }
        [JsonPropertyName("genre_ids")]
        public List<int>? GenreIds { get; set; }
    }

    public class TopSeriesResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<SeriesShort> Results { get; set; }
    }
}
