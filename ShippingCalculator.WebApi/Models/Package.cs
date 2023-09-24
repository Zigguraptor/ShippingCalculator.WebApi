using System.Text.Json.Serialization;

namespace ShippingCalculator.WebApi.Models;

public class Package
{
    [JsonPropertyName("weight")] public int Weight { get; set; }
    [JsonPropertyName("height")] public int? Height { get; set; }
    [JsonPropertyName("length")] public int? Length { get; set; }
    [JsonPropertyName("width")] public int? Width { get; set; }
}
