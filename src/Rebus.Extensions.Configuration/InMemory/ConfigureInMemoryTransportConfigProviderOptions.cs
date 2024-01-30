namespace Rebus.Extensions.Configuration.InMemory;

using Microsoft.Extensions.Options;
using Transport.InMem;

public class ConfigureInMemoryTransportConfigProviderOptions : IPostConfigureOptions<BusOptions>
{
    private readonly Func<string, InMemNetwork> _inMemNetworksFactory;
    private readonly IOptionsMonitor<InMemoryRebusTransportOptions> _optionsMonitor;

    public ConfigureInMemoryTransportConfigProviderOptions(IOptionsMonitor<InMemoryRebusTransportOptions> optionsMonitor, Func<string, InMemNetwork> inMemNetworksFactory)
    {
        _optionsMonitor = optionsMonitor;
        _inMemNetworksFactory = inMemNetworksFactory;
    }

    public void PostConfigure(string name, BusOptions options)
    {
        if (options.Transport?.ProviderName == InMemoryRebusTransportConfigurationProvider.NamedServiceName)
        {
            options.TransportConfigurationProvider = new InMemoryRebusTransportConfigurationProvider(_inMemNetworksFactory, _optionsMonitor);
        }
    }
}
