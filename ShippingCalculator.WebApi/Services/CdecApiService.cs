using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ShippingCalculator.WebApi.Configurations;
using ShippingCalculator.WebApi.Models;

namespace ShippingCalculator.WebApi.Services;

public class CdecApiService : ICdecApiService
{
    private readonly HttpClient _httpClient;
    private readonly CdecApiConfiguration _cdecApiConfiguration;

    public CdecApiService(HttpClient httpClient, ICdecTokenService cdecTokenService,
        CdecApiConfiguration cdecApiConfiguration)
    {
        _httpClient = httpClient;
        _cdecApiConfiguration = cdecApiConfiguration;
        _httpClient.BaseAddress = _cdecApiConfiguration.BaseUri;
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", cdecTokenService.GetToken());
    }

    public async Task<Location> GetLocationByFiasCodeAsync(string fiasCode)
    {
        var parameters = $"?{_cdecApiConfiguration.FiasGuidParameter}={fiasCode}";
        using var response = await _httpClient
            .GetAsync(_cdecApiConfiguration.LocationCitiesUrl + parameters).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"SDEC API request error: {response.ReasonPhrase}");

        await using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        var location = JsonSerializer.Deserialize<IEnumerable<Location>>(responseStream)?.FirstOrDefault();

        if (location == null)
            throw new JsonException();
        if (location.FiasGuid != fiasCode)
            throw new Exception($"Location not found for fiasCode: {fiasCode}");

        return location;
    }

    public async Task<string> CalculateShippingCostAsync(
        IEnumerable<Package> packages, Location sender, Location receiver)
    {
        var request = new RequestToTariffList
        {
            Type = (int)OrderType.Delivery,
            Currency = (int)Currency.RUB,
            FromLocation = sender,
            ToLocation = receiver,
            Packages = packages
        };
        var jsonContent = JsonSerializer.Serialize(request);
        var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var httpResponseMessage = await _httpClient
            .PostAsync(_cdecApiConfiguration.CalculatorTarifflistUrl, stringContent).ConfigureAwait(false);

        return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}
