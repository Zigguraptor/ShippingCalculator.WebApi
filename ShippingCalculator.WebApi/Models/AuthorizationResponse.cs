using System.Text.Json.Serialization;

namespace ShippingCalculator.WebApi.Models;

public class AuthorizationResponse
{
    [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
}
