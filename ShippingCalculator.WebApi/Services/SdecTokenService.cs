using System.Text.Json;
using ShippingCalculator.WebApi.Configurations;
using ShippingCalculator.WebApi.Models;

namespace ShippingCalculator.WebApi.Services;

public class SdecTokenService
{
    private readonly object _lock = new();
    private readonly HttpClient _httpClient;
    private readonly SdecApiConfiguration _sdecApiConfiguration;
    private string _token;
    private DateTime _expiringTime;

    public SdecTokenService(HttpClient httpClient, SdecApiConfiguration sdecApiConfiguration)
    {
        _httpClient = httpClient;
        _sdecApiConfiguration = sdecApiConfiguration;
        _httpClient.BaseAddress = _sdecApiConfiguration.BaseUri;
        _token = GetNewToken();
    }

    public string GetToken()
    {
        if (_expiringTime > DateTime.Now) return _token;

        lock (_lock)
            if (_expiringTime < DateTime.Now)
                _token = GetNewToken();

        return _token;
    }

    private string GetNewToken()
    {
        var parameters =
            $"?grant_type=client_credentials&client_id={_sdecApiConfiguration.ClientId}&client_secret={_sdecApiConfiguration.ClientSecret}";
        using var httpResponseMessage = _httpClient
            .PostAsync(_sdecApiConfiguration.AuthorizationUri + parameters, new StringContent("")).Result;

        if (!httpResponseMessage.IsSuccessStatusCode)
            throw new Exception($"SDEC authorization error: {httpResponseMessage.ReasonPhrase}");

        var responseMessage = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var accessToken = JsonSerializer.Deserialize<AuthorizationResponse>(responseMessage);

        if (accessToken?.AccessToken is null) throw new JsonException(responseMessage);

        _expiringTime = DateTime.Now + TimeSpan.FromMilliseconds((int)(accessToken.ExpiresIn * 0.9d));

        return accessToken.AccessToken;
    }
}
