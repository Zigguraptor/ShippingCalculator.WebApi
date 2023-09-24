namespace ShippingCalculator.WebApi.Configurations;

public static class ConfigurationServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var sdecApiConfiguration = new CdecApiConfiguration();

        configuration.GetSection("CdekApi").Bind(sdecApiConfiguration);

        services.AddSingleton(sdecApiConfiguration);

        return services;
    }
}
