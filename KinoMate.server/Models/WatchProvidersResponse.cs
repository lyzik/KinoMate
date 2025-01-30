using System.Text.Json.Serialization;

public class ProvidersResponse
{
    [JsonPropertyName("results")]
    public Dictionary<string, ProviderDetails> Results { get; set; }
}

public class ProviderDetails
{
    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyName("flatrate")]
    public List<Provider> Flatrate { get; set; }

    [JsonPropertyName("rent")]
    public List<Provider> Rent { get; set; }

    [JsonPropertyName("buy")]
    public List<Provider> Buy { get; set; }
}

public class Provider
{
    [JsonPropertyName("provider_id")]
    public int ProviderId { get; set; }

    [JsonPropertyName("provider_name")]
    public string ProviderName { get; set; }

    [JsonPropertyName("logo_path")]
    public string LogoPath { get; set; }
}