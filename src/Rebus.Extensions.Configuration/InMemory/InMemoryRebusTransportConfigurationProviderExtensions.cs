namespace Rebus.Extensions.Configuration.InMemory;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Transport.InMem;

public static class InMemoryRebusTransportConfigurationProviderExtensions
{
    public static ConfigurationProvidersRegistrationBuilder InMemoryTransport(this ConfigurationProvidersRegistrationBuilder builder, Func<string, InMemNetwork> inMemoryNetworkFactory)
    {
        AddInMemoryTransportConfigureOptions(builder, inMemoryNetworkFactory);
        builder.SetProviderConfigureHook(InMemoryRebusTransportConfigurationProvider.NamedServiceName, ProviderSectionTypeNames.Transport, (busName, transportConfig) => builder.Services.AddOptions<InMemoryRebusTransportOptions>(busName)
            .Bind(transportConfig)
            .ValidateDataAnnotations()
            .ValidateOnStart());
        return builder;
    }

    public static ConfigurationProvidersRegistrationBuilder AddInMemoryTransportConfigureOptions(this ConfigurationProvidersRegistrationBuilder builder, Func<string, InMemNetwork> inMemoryNetworkFactory)
    {
        builder.Services.AddSingleton(inMemoryNetworkFactory);
        builder.Services.AddSingleton<IPostConfigureOptions<BusOptions>, ConfigureInMemoryTransportConfigProviderOptions>();
        return builder;
    }
}
