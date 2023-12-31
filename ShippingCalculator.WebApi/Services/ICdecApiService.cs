﻿using ShippingCalculator.WebApi.Models;

namespace ShippingCalculator.WebApi.Services;

public interface ICdecApiService
{
    public Task<Location> GetLocationByFiasCodeAsync(string fiasCode);
    public Task<string> CalculateShippingCostAsync(IEnumerable<Package> packages, Location sender, Location receiver);
}
