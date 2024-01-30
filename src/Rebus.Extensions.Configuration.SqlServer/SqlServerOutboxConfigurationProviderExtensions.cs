﻿namespace Rebus.Extensions.Configuration.SqlServer;

using Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class SqlServerOutboxConfigurationProviderExtensions
{
    public static ConfigurationProvidersRegistrationBuilder UseSqlServerOutboxProvider(this ConfigurationProvidersRegistrationBuilder builder)
    {
        builder.Services.AddSingleton<IPostConfigureOptions<BusOptions>, ConfigureSqlServerOutboxConfigProviderOptions>();

        builder.SetProviderConfigureHook(SqlServerOutboxConfigurationProvider.NamedServiceName, ProviderSectionTypeNames.Outbox, (busName, transportConfig) => builder.Services.AddOptions<SqlServerOutboxOptions>(busName)
            .Bind(transportConfig)
            .ValidateDataAnnotations()
            .ValidateOnStart());
        return builder;
    }
}
