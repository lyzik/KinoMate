using System.Text.Json.Serialization;

public class WatchProvidersResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("results")]
    public Dictionary<string, WatchProviderCountry> Results { get; set; }
}

public class WatchProviderCountry
{
    [JsonPropertyName("link")]
    public string Link { get; set; }
    [JsonPropertyName("flatrate")]
    public List<WatchProviderInfo> Flatrate { get; set; }
}

public class WatchProviderInfo
{
    [JsonPropertyName("provider_id")]
    public int ProviderId { get; set; }

    [JsonPropertyName("provider_name")]
    public string ProviderName { get; set; }

    [JsonPropertyName("logo_path")]
    public string LogoPath { get; set; }
}