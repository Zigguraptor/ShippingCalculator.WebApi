using System.Text.Json;
using ShippingCalculator.WebApi.Configurations;
using ShippingCalculator.WebApi.Models;

namespace ShippingCalculator.WebApi.Services;

public class CdecTokenService : ICdecTokenService
{
    private readonly object _lock = new();
    private readonly HttpClient _httpClient;
    private readonly CdecApiConfiguration _cdecApiConfiguration;
    private string _token;
    private DateTime _expiringTime;

    public CdecTokenService(HttpClient httpClient, CdecApiConfiguration cdecApiConfiguration)
    {
        _httpClient = httpClient;
        _cdecApiConfiguration = cdecApiConfiguration;
        _httpClient.BaseAddress = _cdecApiConfiguration.BaseUri;
        _token = GetNewToken();
    }

    public string GetToken()
    {
        if (_expiringTime > DateTime.Now) return _token;

        // Если токен истёк, то что бы избежать множественной генерации, мы делаем дабл чек лок.
        // Первый кто залочит сгенерирует токен, остальные выйдут по условию if.
        lock (_lock)
            if (_expiringTime < DateTime.Now)
                _token = GetNewToken();

        return _token;
    }

    private string GetNewToken()
    {
        var parameters =
            $"?grant_type=client_credentials&client_id={_cdecApiConfiguration.ClientId}&client_secret={_cdecApiConfiguration.ClientSecret}";
        using var httpResponseMessage = _httpClient
            .PostAsync(_cdecApiConfiguration.AuthorizationUri + parameters, new StringContent("")).Result;

        if (!httpResponseMessage.IsSuccessStatusCode)
            throw new Exception($"SDEC authorization error: {httpResponseMessage.ReasonPhrase}");

        var responseMessage = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var accessToken = JsonSerializer.Deserialize<AuthorizationResponse>(responseMessage);

        if (accessToken?.AccessToken is null) throw new JsonException(responseMessage);

        // Рассчитываем  когда истечёт токен. Делаем запас 10% времени жизни.
        _expiringTime = DateTime.Now + TimeSpan.FromMilliseconds((int)(accessToken.ExpiresIn * 0.9d));

        return accessToken.AccessToken;
    }
}
