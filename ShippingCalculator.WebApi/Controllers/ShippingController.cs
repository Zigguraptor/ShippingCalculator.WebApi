using Microsoft.AspNetCore.Mvc;
using ShippingCalculator.WebApi.Models;

namespace ShippingCalculator.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ShippingController : ControllerBase
{
    private readonly ILogger<ShippingController> _logger;
    private readonly ISdecApiService _sdecApiService;

    public ShippingController(ILogger<ShippingController> logger, ISdecApiService sdecApiService)
    {
        _logger = logger;
        _sdecApiService = sdecApiService;
    }

    [HttpGet]
    public async Task<IActionResult> CalculateShippingCost(
        int weightGrams, int lengthMm, int widthMm, int heightMm,
        string senderCityFias, string receiverCityFias)
    {
        try
        {
            var senderLocation = await _sdecApiService.GetLocationByFiasCodeAsync(senderCityFias);
            // Делаем второй запрос только если коды разные.
            var receiverLocation = senderCityFias != receiverCityFias
                ? await _sdecApiService.GetLocationByFiasCodeAsync(receiverCityFias)
                : senderLocation;
            var package = new Package
            {
                weight = weightGrams,
                height = (int)Math.Ceiling(heightMm / 10d),
                length = (int)Math.Ceiling(lengthMm / 10d),
                width = (int)Math.Ceiling(widthMm / 10d)
            };
            var costList =
                await _sdecApiService.CalculateShippingCostAsync(new[] { package }, senderLocation, receiverLocation);
            return new JsonResult(costList);
        }
        catch (Exception e)
        {
            _logger.LogError("{exception}", e.ToString());
            return StatusCode(500);
        }
    }
}
