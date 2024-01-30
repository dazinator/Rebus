namespace Rebus.Extensions.Configuration.FileSystem;

using Core;
using Microsoft.Extensions.Options;
using ServiceBus;

public class ConfigureServiceBusTransportConfigProviderOptions: IPostConfigureOptions<BusOptions>
{
    private readonly IOptionsMonitor<ServiceBusRebusTransportOptions> _optionsMonitor;

    public ConfigureServiceBusTransportConfigProviderOptions(IOptionsMonitor<ServiceBusRebusTransportOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public void PostConfigure(string name, BusOptions options)
    {
        if (options.Transport?.ProviderName == ServiceBusRebusTransportConfigurationProvider.NamedServiceName)
        {
            options.TransportConfigurationProvider = new ServiceBusRebusTransportConfigurationProvider(_optionsMonitor);
        }
    }
}
