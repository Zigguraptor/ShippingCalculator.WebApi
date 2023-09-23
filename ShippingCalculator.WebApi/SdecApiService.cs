using Microsoft.Net.Http.Headers;
using ShippingCalculator.WebApi.Configurations;

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
}
