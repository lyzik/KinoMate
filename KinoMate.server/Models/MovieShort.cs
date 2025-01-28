using System.Text.Json.Serialization;

namespace KinoMate.server.Models
{
    public class MovieShort
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }
        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("popularity")]
        public double? Popularity { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }
        [JsonPropertyName("genre_ids")]
        public List<int>? GenreIds { get; set; }

    }

    public class PopularMoviesResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<MovieShort> Results { get; set; }
    }
}
