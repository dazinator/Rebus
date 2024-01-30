namespace Rebus.Extensions.Configuration;

using Core.Infrastructure.Rebus;
using Dazinator.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class RebusConfigurationProviderOptionsBuilder
{
    private readonly Dictionary<string, Action<string, IConfiguration>> _outboxConfigurationProviderCallbacks = new();

    private readonly Dictionary<string, Action<string, IConfiguration>> _transportConfigurationProviderCallbacks = new();

    public RebusConfigurationProviderOptionsBuilder(IServiceCollection services) => Services = services;

    public IServiceCollection Services { get; }

    public RebusConfigurationProviderOptionsBuilder AddTransportConfigurationProvider<TProvider>(string name)
        where TProvider : RebusTransportConfigurationProvider
    {
        Services.AddSingleton<RebusTransportConfigurationProvider, TProvider>(name);
        return this;
    }

    public RebusConfigurationProviderOptionsBuilder AddOutboxConfigurationProvider<TProvider>(string name)
        where TProvider : RebusOutboxConfigurationProvider
    {
        Services.AddSingleton<RebusOutboxConfigurationProvider, TProvider>(name);
        return this;
    }


    public RebusConfigurationProviderOptionsBuilder SetTransportProvider(string providerName, Action<string, IConfiguration> configureHook)
    {
        _transportConfigurationProviderCallbacks[providerName] = configureHook;
        return this;
    }

    public void InvokeTransportProvider(string providerName, string busName, IConfigurationSection providerConfig)
    {
        if (_transportConfigurationProviderCallbacks.TryGetValue(providerName, out var configureHook))
        {
            configureHook(busName, providerConfig);
            return;
        }

        throw new NotSupportedException($"Invalid provider: {providerName}");
    }

    public RebusConfigurationProviderOptionsBuilder SetOutboxProvider(string providerName, Action<string, IConfiguration> configureHook)
    {
        _outboxConfigurationProviderCallbacks[providerName] = configureHook;
        return this;
    }

    public void InvokeOutboxProvider(string providerName, string busName, IConfigurationSection providerConfig)
    {
        if (_outboxConfigurationProviderCallbacks.TryGetValue(providerName, out var configureHook))
        {
            configureHook(busName, providerConfig);
            return;
        }

        throw new NotSupportedException($"Invalid provider: {providerName}");
    }
}
