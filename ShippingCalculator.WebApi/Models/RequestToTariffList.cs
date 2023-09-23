namespace ShippingCalculator.WebApi.Models;

public class RequestToTariffList
{
    public int type { get; set; }
    public int currency { get; set; }
    public Location from_location { get; set; }
    public Location to_location { get; set; }
    public IEnumerable<Package> packages { get; set; }
}
