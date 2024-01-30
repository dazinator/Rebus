namespace Rebus.Extensions.Configuration;

using Bus;
using Config;
using Core;
using Core.Infrastructure.Rebus;
using DataBus.InMem;
using Dazinator.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serialization.Json;
using Transport;
using Transport.InMem;

public static class ServiceCollectionExtensions
{
    private const string DefaultRebusConfigKey = "Rebus";

    /// <summary>
    ///     Configures all rebus buses from the <see cref="IConfiguration" /> section: "Rebus".
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <param name="configureRebus"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static IServiceCollection AddRebusFromConfiguration(
        this IServiceCollection services,
        IConfiguration configSection,
        Action<RebusConfigurationProviderOptionsBuilder> configureProviders,
        Func<IServiceProvider, string, BusOptions, RebusConfigurer, RebusConfigurer> configureRebus = null
    )
    {
        // var rebus = config.GetSection(DefaultRebusConfigKey);
        var rebusOptions = new RebusOptions();
        configSection.Bind(rebusOptions);

        var builder = new RebusConfigurationProviderOptionsBuilder(services);
        configureProviders?.Invoke(builder);

        AddRebus(services, rebusOptions, (busName, busOptions) =>
        {
            var transportOptions = busOptions.Transport;
            if (string.IsNullOrWhiteSpace(transportOptions.ProviderName))
            {
                throw new Exception("No transport provider configured. Check config key: Rebus:Transport:ProviderName");
            }

            var busConfig = configSection.GetSection($"Buses:{busName}");
            var transportSection = busConfig.GetSection("Transport");
            transportSection.Bind(transportOptions);

            var transportConfig = transportSection.GetSection($"Providers:{transportOptions.ProviderName}");
            builder.InvokeTransportProvider(transportOptions.ProviderName, busName, transportConfig);

            var outboxOptions = busOptions.Outbox;
            if (!string.IsNullOrWhiteSpace(outboxOptions?.ProviderName))
            {
                var outboxConfigSection = busConfig.GetSection("Outbox");
                outboxConfigSection.Bind(outboxOptions);

                var outboxProviderConfig = outboxConfigSection.GetSection($"Providers:{outboxOptions.ProviderName}");
                builder.InvokeOutboxProvider(outboxOptions.ProviderName, busName, outboxProviderConfig);
            }

            AddRebusBus(services, busName, busOptions,
                (sp, r) => configureRebus?.Invoke(sp, busName, busOptions, r) ?? r);
        });

        return services;
    }

    private static void ConfigureTransportFromProvider(
        StandardConfigurer<ITransport> standardConfigurer,
        IServiceProvider sp, string busName, BusOptions busOptions
    )
    {
        using var scoped = sp.CreateScope(); // as cannot resolve named services from root scope.
        var transportOptions = busOptions.Transport;
        var transportConfigurer =
            scoped.ServiceProvider.GetNamed<RebusTransportConfigurationProvider>(transportOptions.ProviderName);
        transportConfigurer.ConfigureTransport(busName, busOptions, standardConfigurer);
    }

    private static void ConfigureOutboxFromProvider(
        RebusConfigurer standardConfigurer,
        IServiceProvider sp, string busName, BusOptions busOptions
    )
    {
        using var scoped = sp.CreateScope(); // as cannot resolve named services from root scope.
        var optionsProviderName = busOptions.Outbox?.ProviderName;
        if (string.IsNullOrWhiteSpace(optionsProviderName))
        {
            return;
        }

        var optionsConfigurer = scoped.ServiceProvider.GetNamed<RebusOutboxConfigurationProvider>(optionsProviderName);
        optionsConfigurer.ConfigureOutbox(busName, busOptions, standardConfigurer);
    }

    public static IServiceCollection AddRebus(
            this IServiceCollection services,
            RebusOptions options,
            Action<string, BusOptions> configureBus
        )
        //  IConfiguration config, Func<RebusConfigurer, RebusConfigurer> configureRebus = null)
    {
        // var rebus = config.GetSection(DefaultRebusConfigKey);
        // add any in memory networks and data stores?
        services.AddNamed<InMemNetwork>(n =>
        {
            foreach (var networkName in options.InMemoryNetworks)
            {
                n.AddSingleton(networkName, new InMemNetwork());
            }
        });

        services.AddNamed<InMemDataStore>(n =>
        {
            foreach (var datastoreName in options.InMemoryDataStores)
            {
                n.AddSingleton(datastoreName, new InMemDataStore());
            }
        });

        foreach (var bus in options.Buses)
        {
            var busName = bus.Key;
            var busOptions = bus.Value;
            if (bus.Key == options.DefaultBus)
            {
                busOptions.IsDefault = true;
                // busName = string.Empty;
            }

            configureBus(busName, bus.Value);
            //  var busConfig = rebus.GetSection($"Buses:{bus}");
            // AddBus(services, busName, busConfig, configureRebus);
        }

        return services;
    }

    /// <summary>
    ///     Adds a rebus <see cref="IBus" /> and configures it from matching named options.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="busName"></param>
    /// <param name="busOptions"></param>
    /// <param name="configureRebusCallback"></param>
    /// <remarks>
    ///     Should call  services.Configure`InMemoryRebusTransportOptions`(busName, ..) or
    ///     services.Configure`ServiceBusRebusTransportOptions`(busName, transportConfig);
    ///     to configure the providers transport options for the bus before calling this method
    /// </remarks>
    public static void AddRebusBus(
        this IServiceCollection services,
        string busName,
        BusOptions busOptions,
        Func<IServiceProvider, RebusConfigurer, RebusConfigurer> configureRebusCallback
    ) =>
        AddRebusBus(services, busName, busOptions.IsDefault, busOptions, configureRebusCallback);


    private static void AddRebusBus(
        this IServiceCollection services,
        string busName,
        bool isDefaultBus,
        BusOptions busOptions,
        Func<IServiceProvider, RebusConfigurer, RebusConfigurer> configureRebusCallback
    ) =>
        //var isDefaultBus = string.IsNullOrEmpty(busName || busOptions.
        services.AddRebus((configure, sp) =>
        {
            // var network = sp.GetNamed<InMemNetwork>(name);
            // var dataStore = sp.GetNamed<InMemDataStore>(name);

            configure = configure
                .Transport(t =>
                    // based on the provider name, we want to delegate the configuration of the rebus transport to a provider that can understand how to apply its own config.
                    ConfigureTransportFromProvider(t, sp, busName, busOptions))
                //.DataBus(d => d.StoreInMemory(dataStore))
                .Serialization(s => s.UseNewtonsoftJson());

            ConfigureOutboxFromProvider(configure, sp, busName, busOptions);

            return configureRebusCallback?.Invoke(sp, configure);
        }, key: busName, isDefaultBus: isDefaultBus);
}
