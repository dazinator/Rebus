namespace Rebus.Extensions.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ConfigurationProvidersRegistrationBuilder
{
    private readonly Dictionary<string, Action<string, IConfiguration>> _transportConfigurationProviderCallbacks = new();

    public IServiceCollection Services { get; }

    public ConfigurationProvidersRegistrationBuilder(IServiceCollection serviceCollection)
    {
        Services = serviceCollection;
    }

    public ConfigurationProvidersRegistrationBuilder SetProviderConfigureHook(string providerName, ProviderSectionTypeNames sectionType, Action<string, IConfiguration> configureHook)
    {
        var key = $"{providerName}:{sectionType}";
        _transportConfigurationProviderCallbacks[key] = configureHook;
        return this;
    }

    public void InvokeProviderConfigureHook(string providerName, ProviderSectionTypeNames sectionType, string busName, IConfigurationSection providerConfig)
    {
        var key = $"{providerName}:{sectionType}";
        if (_transportConfigurationProviderCallbacks.TryGetValue(key, out var configureHook))
        {
            configureHook(busName, providerConfig);
            return;
        }

        throw new NotSupportedException($"Invalid provider: {providerName}");
    }
}
