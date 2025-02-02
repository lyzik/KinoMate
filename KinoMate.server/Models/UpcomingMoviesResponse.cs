﻿using System.Text.Json.Serialization;

namespace KinoMate.server.Models
{
    public class UpcomingMoviesResponse
    {
        [JsonPropertyName("results")]
        public List<UpcomingMovie> Results { get; set; }
    }

    public class UpcomingMovie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<int> GenreIds { get; set; }
    }
}
