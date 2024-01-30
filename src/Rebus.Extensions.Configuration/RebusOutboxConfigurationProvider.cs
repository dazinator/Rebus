namespace Core.Infrastructure.Rebus;

using global::Rebus.Config;

public abstract class RebusOutboxConfigurationProvider
{
    public abstract void ConfigureOutbox(string busName, BusOptions transportOptions, RebusConfigurer configurer);
}
