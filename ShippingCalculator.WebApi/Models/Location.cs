using System.Text.Json.Serialization;

namespace ShippingCalculator.WebApi.Models;

public class Location
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("fias_guid")] public string? FiasGuid { get; set; }
}
