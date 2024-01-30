namespace Rebus.Extensions.Configuration.InMemory;

using Config;
using Microsoft.Extensions.Options;
using Transport;
using Transport.InMem;

public class InMemoryRebusTransportConfigurationProvider : RebusTransportConfigurationProvider
{
    public const string NamedServiceName = "InMemory";
    private readonly Func<string, InMemNetwork> _getNamedNetwork;
    private readonly IOptionsMonitor<InMemoryRebusTransportOptions> _options;

    public InMemoryRebusTransportConfigurationProvider(
        Func<string, InMemNetwork> getNamedNetwork,
        IOptionsMonitor<InMemoryRebusTransportOptions> options
    )
    {
        _getNamedNetwork = getNamedNetwork;
        _options = options;
    }

    public override void ConfigureTransport(
        string busName,
        BusOptions busOptions,
        StandardConfigurer<ITransport> configurer
    )
    {
        var options = string.IsNullOrWhiteSpace(busName) ? _options.CurrentValue : _options.Get(busName);
        var transportOptions = busOptions.Transport;
        var networkName = options.NetworkName;
        var registerForSubscriptionStorage = options.RegisterForSubscriptionStorage;
        // configurer.
        var network = _getNamedNetwork(networkName);
        //  var dataStore = _serviceProvider.GetNamed<InMemDataStore>(name);
        configurer.UseInMemoryTransport(network, busOptions.GetInputQueueName(),
            registerForSubscriptionStorage);
        // .DataBus(d => d.StoreInMemory(dataStore))
        // .Serialization(s => s.UseNewtonsoftJson());
    }
}
