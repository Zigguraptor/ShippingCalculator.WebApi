using System.Text;
using Microsoft.AspNetCore.Mvc;
using ShippingCalculator.WebApi.Models;
using ShippingCalculator.WebApi.Services;

namespace ShippingCalculator.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ShippingController : ControllerBase
{
    private readonly ILogger<ShippingController> _logger;
    private readonly ICdecApiService _cdecApiService;

    public ShippingController(ILogger<ShippingController> logger, ICdecApiService cdecApiService)
    {
        _logger = logger;
        _cdecApiService = cdecApiService;
    }

    [HttpGet]
    public async Task<IActionResult> CalculateShippingCost(
        int weightGrams, int lengthMm, int widthMm, int heightMm,
        string senderCityFias, string receiverCityFias)
    {
        try
        {
            var senderLocation = await _cdecApiService.GetLocationByFiasCodeAsync(senderCityFias);
            // Делаем второй запрос только если коды разные.
            var receiverLocation = senderCityFias != receiverCityFias
                ? await _cdecApiService.GetLocationByFiasCodeAsync(receiverCityFias)
                : senderLocation;
            // Перевод миллиметров в сантиметры.
            var package = new Package
            {
                Weight = weightGrams,
                HeightCm = (int)Math.Ceiling(heightMm / 10d),
                LengthCm = (int)Math.Ceiling(lengthMm / 10d),
                WidthCm = (int)Math.Ceiling(widthMm / 10d)
            };
            var costList =
                await _cdecApiService.CalculateShippingCostAsync(new[] { package }, senderLocation, receiverLocation);

            return Content(costList, "application/json", Encoding.UTF8);
        }
        catch (Exception e)
        {
            _logger.LogError("{exception}", e.ToString());
            return StatusCode(500);
        }
    }
}
