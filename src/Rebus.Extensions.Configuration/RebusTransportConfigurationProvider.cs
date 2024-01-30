namespace Rebus.Extensions.Configuration;

using Rebus.Config;
using Rebus.Transport;

public abstract class RebusTransportConfigurationProvider
{
    public abstract void ConfigureTransport(string busName, BusOptions transportOptions, StandardConfigurer<ITransport> configurer);
}
