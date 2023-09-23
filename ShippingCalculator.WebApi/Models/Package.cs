namespace ShippingCalculator.WebApi.Models;

public class Package
{
    public int weight { get; set; }
    public int? height { get; set; }
    public int? length { get; set; }
    public int? width { get; set; }
}
