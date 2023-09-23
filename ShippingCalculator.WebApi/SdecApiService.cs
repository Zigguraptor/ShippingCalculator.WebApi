using System.Text.Json;
using Microsoft.Net.Http.Headers;
using ShippingCalculator.WebApi.Configurations;
using ShippingCalculator.WebApi.Models;

namespace ShippingCalculator.WebApi;

public class SdecApiService
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
}
