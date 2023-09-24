using System.Text.Json.Serialization;

namespace ShippingCalculator.WebApi.Models;

public class Package
{
    [JsonPropertyName("weight")] public int Weight { get; set; }
    [JsonPropertyName("height")] public int? HeightCm { get; set; }
    [JsonPropertyName("length")] public int? LengthCm { get; set; }
    [JsonPropertyName("width")] public int? WidthCm { get; set; }
}
