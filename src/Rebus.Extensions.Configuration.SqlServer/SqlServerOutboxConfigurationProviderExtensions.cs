namespace Rebus.Extensions.Configuration.SqlServer;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class SqlServerOutboxConfigurationProviderExtensions
{
    public static ConfigurationProvidersRegistrationBuilder SqlServerOutbox(this ConfigurationProvidersRegistrationBuilder builder)
    {
        AddSqlServerOutboxConfigureOptions(builder);
        builder.SetProviderConfigureHook(SqlServerOutboxConfigurationProvider.NamedServiceName, ProviderSectionTypeNames.Outbox, (busName, transportConfig) => builder.Services.AddOptions<SqlServerOutboxOptions>(busName)
            .Bind(transportConfig)
            .ValidateDataAnnotations()
            .ValidateOnStart());
        return builder;
    }

    private static ConfigurationProvidersRegistrationBuilder AddSqlServerOutboxConfigureOptions(this ConfigurationProvidersRegistrationBuilder builder)
    {
        builder.Services.AddSingleton<IPostConfigureOptions<BusOptions>, ConfigureSqlServerOutboxConfigProviderOptions>();
        return builder;
    }
}
