namespace Rebus.Extensions.Configuration;

using Rebus.Config;

public abstract class RebusOutboxConfigurationProvider
{
    public abstract void ConfigureOutbox(string busName, BusOptions transportOptions, RebusConfigurer configurer);
}
