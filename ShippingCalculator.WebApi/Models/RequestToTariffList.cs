using System.Text.Json.Serialization;

namespace ShippingCalculator.WebApi.Models;

public class RequestToTariffList
{
    [JsonPropertyName("type")] public int Type { get; set; }
    [JsonPropertyName("currency")] public int Currency { get; set; }
    [JsonPropertyName("from_location")] public Location? FromLocation { get; set; }
    [JsonPropertyName("to_location")] public Location? ToLocation { get; set; }
    [JsonPropertyName("packages")] public IEnumerable<Package>? Packages { get; set; }
}
