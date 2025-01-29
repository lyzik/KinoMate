using System.Text.Json.Serialization;

public class WatchProvidersResponse
{
    [JsonPropertyName("results")]
    public Dictionary<string, WatchProviderCountry> Results { get; set; }
}

public class WatchProviderCountry
{
    [JsonPropertyName("flatrate")]
    public List<StreamingProvider> Flatrate { get; set; }
}

public class StreamingProvider
{
    [JsonPropertyName("provider_name")]
    public string ProviderName { get; set; }

    [JsonPropertyName("logo_path")]
    public string LogoPath { get; set; }
}
