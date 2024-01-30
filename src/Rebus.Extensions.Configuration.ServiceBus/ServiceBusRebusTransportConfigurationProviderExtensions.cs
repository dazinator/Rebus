namespace Rebus.Extensions.Configuration.ServiceBus;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceBusRebusTransportConfigurationProviderExtensions
{
    public static RebusConfigurationProviderOptionsBuilder UseServiceBusTransportProvider(this RebusConfigurationProviderOptionsBuilder builder)
    {
        builder.AddTransportConfigurationProvider<ServiceBusRebusTransportConfigurationProvider>(ServiceBusRebusTransportConfigurationProvider.NamedServiceName);

        builder.SetTransportProvider(ServiceBusRebusTransportConfigurationProvider.NamedServiceName, (busName, transportConfig) => builder.Services.AddOptions<ServiceBusRebusTransportOptions>(busName)
                .Bind(transportConfig)
                .ValidateDataAnnotations()
                .ValidateOnStart());
        return builder;
    }
}
