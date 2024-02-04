namespace Rebus.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static RebusRegistrationBuilder  AddRebusConfigurationProviders(this IServiceCollection services, Action<ConfigurationProvidersRegistrationBuilder>? configureProviders = null)
    {
        var builder = new ConfigurationProvidersRegistrationBuilder(services);
        configureProviders?.Invoke(builder);

        var nextBuilder = builder.Build();
        return nextBuilder;
    }
}
