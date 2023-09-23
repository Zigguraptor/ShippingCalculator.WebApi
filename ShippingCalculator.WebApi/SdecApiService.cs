using System.Text;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using ShippingCalculator.WebApi.Configurations;
using ShippingCalculator.WebApi.Models;

namespace ShippingCalculator.WebApi;

public class SdecApiService : ISdecApiService
{
    private readonly HttpClient _httpClient;
    private readonly SdecApiConfiguration _sdecApiConfiguration;

    public SdecApiService(HttpClient httpClient, SdecApiConfiguration sdecApiConfiguration)
    {
        _httpClient = httpClient;
        _sdecApiConfiguration = sdecApiConfiguration;

        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, sdecApiConfiguration.Token);
        _httpClient.BaseAddress = _sdecApiConfiguration.BaseUri;
    }

    public async Task<Location> GetLocationByFiasCodeAsync(string fiasCode)
    {
        var parameters = $"?{_sdecApiConfiguration.FiasGuidParameter}={fiasCode}";
        using var response = await _httpClient
            .GetAsync(_sdecApiConfiguration.LocationCitiesUrl + parameters).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"SDEC API request error: {response.ReasonPhrase}");

        await using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<IEnumerable<Location>>(responseStream)?.FirstOrDefault() ??
               throw new JsonException();
    }

    public async Task<string> CalculateShippingCostAsync(
        IEnumerable<Package> packages, Location sender, Location receiver)
    {
        var request = new RequestToTariffList
        {
            type = (int)OrderType.Delivery,
            currency = (int)Currency.RUB,
            from_location = sender,
            to_location = receiver,
            packages = packages
        };
        var jsonContent = JsonSerializer.Serialize(request);
        var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var httpResponseMessage = await _httpClient
            .PostAsync(_sdecApiConfiguration.CalculatorTarifflistUrl, stringContent).ConfigureAwait(false);

        return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}
