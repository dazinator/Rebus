namespace Core.Infrastructure.Rebus;

using global::Rebus.Config;
using global::Rebus.Transport;

public abstract class RebusTransportConfigurationProvider
{
    public abstract void ConfigureTransport(string busName, BusOptions transportOptions, StandardConfigurer<ITransport> configurer);
}
