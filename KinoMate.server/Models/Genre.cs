using System.Text.Json.Serialization;

namespace KinoMate.server.Models
{
    public class Genre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
    public class GenresResponse
    {
        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; }
    }

}
