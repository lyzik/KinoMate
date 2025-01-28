using System.Text.Json.Serialization;

namespace KinoMate.server.Models
{
    public class VideosResponse
    {
        [JsonPropertyName("results")]
        public List<VideoResult> Results { get; set; }
    }

    public class VideoResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("site")]
        public string Site { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
