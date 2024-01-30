namespace Rebus.Extensions.Configuration;

using Config;
using DataBus.InMem;
using Dazinator.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serialization.Json;
using Transport.InMem;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRebusFromConfiguration(
        this IServiceCollection services,
        IConfiguration config,
        Action<ConfigurationProvidersRegistrationBuilder>? configureProviders = null
    )
    {
        // I need to get the bus names and which is the default bus.

        var rebusOptions = new RebusOptions();
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

        var builder = new ConfigurationProvidersRegistrationBuilder(services);
        configureProviders?.Invoke(builder);

        foreach (var bus in rebusOptions.Buses)
        {
            var busName = bus.Key;
            var busConfig = config.GetSection("Buses:" + busName);
            services.Configure<BusOptions>(busName, busConfig);

            var transport = bus.Value.Transport;

            var transportSection = busConfig.GetSection("Transport");
            var transportConfig = transportSection.GetSection($"Providers:{transport.ProviderName}");

            builder.InvokeProviderConfigureHook(transport.ProviderName, ProviderSectionTypeNames.Transport, busName, transportConfig);

            if (!string.IsNullOrWhiteSpace(bus.Value.Outbox?.ProviderName))
            {
                var outboxProviderName = bus.Value.Outbox.ProviderName;
                var outboxConfigSection = busConfig.GetSection("Outbox");
                var outboxProviderConfig = outboxConfigSection.GetSection($"Providers:{outboxProviderName}");
                builder.InvokeProviderConfigureHook(outboxProviderName, ProviderSectionTypeNames.Outbox, busName, outboxProviderConfig);
            }
        }

        var busNames = rebusOptions.Buses.Select(a => a.Key).ToArray();
        AddRebusFromOptions(services, rebusOptions.DefaultBus, busNames);
        return services;
    }

    public static IServiceCollection AddRebusFromOptions(this IServiceCollection services, string defaultBusName, string[] busNames)
    {
        // I need to get the bus names and which is the default bus.
        foreach (var bus in busNames)
        {
            services.AddRebus((configure, sp) =>
            {
                // By resolving the options here, we can allow IConfigureOptions<BusOptions> to run which is registered by additional providers, and they can configure themselves on the BusOptions, and we can delegate aspects of the configuration to them.
                var busOptions = sp.GetRequiredService<IOptionsMonitor<BusOptions>>();
                var namedBusOptions = busOptions.Get(bus);

                configure = configure
                    .Transport(t => namedBusOptions.TransportConfigurationProvider?.ConfigureTransport(bus, namedBusOptions, t))
                    .Serialization(s => s.UseNewtonsoftJson());

                namedBusOptions.OutboxConfigurationProvider?.ConfigureOutbox(bus, namedBusOptions, configure);
                return configure;
            }, key: bus, isDefaultBus: bus == defaultBusName);
        }

        return services;
    }
}
