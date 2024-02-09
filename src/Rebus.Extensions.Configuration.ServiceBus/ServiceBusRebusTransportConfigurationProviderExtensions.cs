namespace Rebus.Extensions.Configuration.ServiceBus;

using FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class ServiceBusRebusTransportConfigurationProviderExtensions
{
    public static ConfigurationProvidersRegistrationBuilder ServiceBusTransport(this ConfigurationProvidersRegistrationBuilder builder)
    {
        AddServiceBusConfigureOptions(builder);
        builder.SetProviderConfigureHook(ServiceBusRebusTransportConfigurationProvider.NamedServiceName, ProviderSectionTypeNames.Transport, (busName, transportConfig) => builder.Services.AddOptions<ServiceBusRebusTransportOptions>(busName)
            .Bind(transportConfig)
            .ValidateDataAnnotations()
            .ValidateOnStart());
        return builder;
    }

    private static ConfigurationProvidersRegistrationBuilder AddServiceBusConfigureOptions(this ConfigurationProvidersRegistrationBuilder builder)
    {
        builder.Services.AddSingleton<IPostConfigureOptions<BusOptions>, ConfigureServiceBusTransportConfigProviderOptions>();
        return builder;
    }


}
