namespace Rebus.Extensions.Configuration.InMemory;

using Microsoft.Extensions.DependencyInjection;

public static class InMemoryRebusTransportConfigurationProviderExtensions
{
    public static RebusConfigurationProviderOptionsBuilder UseInMemoryTransportProvider(this RebusConfigurationProviderOptionsBuilder builder)
    {
        builder.AddTransportConfigurationProvider<InMemoryRebusTransportConfigurationProvider>(InMemoryRebusTransportConfigurationProvider.NamedServiceName);

        builder.SetTransportProvider(InMemoryRebusTransportConfigurationProvider.NamedServiceName, (busName, transportConfig) =>
        {
            builder.Services.AddOptions<InMemoryRebusTransportOptions>(busName)
                .Bind(transportConfig)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        });
        return builder;
    }
}
