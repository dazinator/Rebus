namespace Rebus.Extensions.Configuration;

using Config;
using DataBus.InMem;
using Dazinator.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serialization.Json;
using Transport.InMem;

public class RebusRegistrationBuilder
{
    private readonly ConfigurationProvidersRegistrationBuilder _builder;

    public RebusRegistrationBuilder(ConfigurationProvidersRegistrationBuilder builder)
    {
        this._builder = builder;
    }

    public IServiceCollection AddRebus(IConfiguration config)
    {
        // I need to get the bus names and which is the default bus.
        var services = _builder.Services;

        var rebusOptions = new RebusOptions();
        services.Configure<RebusOptions>(config);

        config.Bind(rebusOptions);
        var buses = new List<string>();
        buses.Add(rebusOptions.DefaultBus);

        services.AddNamed<InMemNetwork>(n =>
        {
            foreach (var networkName in rebusOptions.InMemoryNetworks)
            {
                n.AddSingleton(networkName, new InMemNetwork());
            }
        });

        services.AddNamed<InMemDataStore>(n =>
        {
            foreach (var datastoreName in rebusOptions.InMemoryDataStores)
            {
                n.AddSingleton(datastoreName, new InMemDataStore());
            }
        });

        foreach (var bus in rebusOptions.Buses)
        {
            var busName = bus.Key;
            var busConfig = config.GetSection("Buses:" + busName);
            services.Configure<BusOptions>(busName, busConfig);

            var transport = bus.Value.Transport;

            var transportSection = busConfig.GetSection("Transport");
            var transportConfig = transportSection.GetSection($"Providers:{transport.ProviderName}");

            _builder.InvokeProviderConfigureHook(transport.ProviderName, ProviderSectionTypeNames.Transport, busName, transportConfig);

            if (!string.IsNullOrWhiteSpace(bus.Value.Outbox?.ProviderName))
            {
                var outboxProviderName = bus.Value.Outbox.ProviderName;
                var outboxConfigSection = busConfig.GetSection("Outbox");
                var outboxProviderConfig = outboxConfigSection.GetSection($"Providers:{outboxProviderName}");
                _builder.InvokeProviderConfigureHook(outboxProviderName, ProviderSectionTypeNames.Outbox, busName, outboxProviderConfig);
            }

            AddRebus(busName, busName == rebusOptions.DefaultBus);
        }

        return services;
    }

    /// <summary>
    /// Adds a rebus bus, and configures it based on provided <see cref="BusOptions"/>. If you don't provide a factory then by default a named BusOptions (with the same name as the bus name) is expected to be configured with the <see cref="IServiceCollection"/> independently.
    /// </summary>
    /// <param name="busNameAndOptionsName"></param>
    /// <param name="isDefault"></param>
    /// <returns></returns>
    public IServiceCollection AddRebus(string busNameAndOptionsName, bool isDefault, Func<IServiceProvider, BusOptions>? getBusOptions = null)
    {
        // I need to get the bus names and which is the default bus.
        getBusOptions ??= (s =>
        {
            var busOptions = s.GetRequiredService<IOptionsMonitor<BusOptions>>();
            return busOptions.Get(busNameAndOptionsName);
        });


        var services = _builder.Services;
        services.AddRebus((configure, sp) =>
        {
            // By resolving the options here, we can allow IConfigureOptions<BusOptions> to run which is registered by additional providers, and they can configure themselves on the BusOptions, and we can delegate aspects of the configuration to them.
            var busOptions = getBusOptions.Invoke(sp);


            configure = configure
                .Transport(t => busOptions.TransportConfigurationProvider?.ConfigureTransport(busNameAndOptionsName, busOptions, t))
                .Serialization(s => s.UseNewtonsoftJson());

            busOptions.OutboxConfigurationProvider?.ConfigureOutbox(busNameAndOptionsName, busOptions, configure);

            var callbackOptions = sp.GetRequiredService<IOptionsMonitor<RebusConfigureActions>>();
            var globalCallbacks = callbackOptions.CurrentValue;

            var context = new BusConfigurationContext() { Configurer = configure, Name = busNameAndOptionsName, ServiceProvider = sp, ConfiguredOptions = busOptions };
            globalCallbacks?.InvokeCallbacks(context);

            var thisBusCallbacks = callbackOptions.Get(busNameAndOptionsName);
            thisBusCallbacks?.InvokeCallbacks(context);

            return configure;
        }, key: busNameAndOptionsName, isDefaultBus: isDefault);


        return services;
    }
}

public class ConfigurationProvidersRegistrationBuilder
{
    private readonly Dictionary<string, Action<string, IConfiguration>> _transportConfigurationProviderCallbacks = new();

    public ConfigurationProvidersRegistrationBuilder(IServiceCollection serviceCollection) => Services = serviceCollection;

    public IServiceCollection Services { get; }

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

    public ConfigurationProvidersRegistrationBuilder UseConfigureCallback(string busName, Func<BusConfigurationContext, RebusConfigurer> configureBus)
    {
        Services.Configure<RebusConfigureActions>(busName, (a) =>
        {
            a.Add(configureBus);
        });
        return this;
    }

    public ConfigurationProvidersRegistrationBuilder UseConfigureCallback(Func<BusConfigurationContext, RebusConfigurer> configureBus)
    {
        Services.Configure<RebusConfigureActions>((a) =>
        {
            a.Add(configureBus);
        });
        return this;
    }

    internal RebusRegistrationBuilder Build()
    {
        return new RebusRegistrationBuilder(this);
    }
}
