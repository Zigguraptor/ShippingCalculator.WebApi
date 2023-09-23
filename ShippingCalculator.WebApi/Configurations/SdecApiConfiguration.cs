namespace ShippingCalculator.WebApi.Configurations;

public class SdecApiConfiguration
{
    public Uri BaseUri { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string LocationCitiesUrl { get; set; } = null!;
    public string FiasGuidParameter { get; set; } = null!;
    public string CalculatorTarifflistUrl { get; set; } = null!;
}
