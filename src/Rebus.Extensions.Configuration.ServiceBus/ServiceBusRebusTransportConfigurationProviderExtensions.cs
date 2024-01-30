namespace Rebus.Extensions.Configuration.ServiceBus;

using Core;
using FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class ServiceBusRebusTransportConfigurationProviderExtensions
{
    public static ConfigurationProvidersRegistrationBuilder UseServiceBusTransportProvider(this ConfigurationProvidersRegistrationBuilder builder)
    {
        builder.Services.AddSingleton<IPostConfigureOptions<BusOptions>, ConfigureServiceBusTransportConfigProviderOptions>();

        builder.SetProviderConfigureHook(ServiceBusRebusTransportConfigurationProvider.NamedServiceName, ProviderSectionTypeNames.Outbox, (busName, transportConfig) => builder.Services.AddOptions<ServiceBusRebusTransportOptions>(busName)
            .Bind(transportConfig)
            .ValidateDataAnnotations()
            .ValidateOnStart());
        return builder;
    }
}
