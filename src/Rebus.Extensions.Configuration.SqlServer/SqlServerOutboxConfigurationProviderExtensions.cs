namespace Rebus.Extensions.Configuration.SqlServer;

using Microsoft.Extensions.DependencyInjection;

public static class SqlServerOutboxConfigurationProviderExtensions
{
    public static RebusConfigurationProviderOptionsBuilder UseSqlServerOutboxProvider(this RebusConfigurationProviderOptionsBuilder builder)
    {
        builder.AddOutboxConfigurationProvider<SqlServerOutboxConfigurationProvider>(SqlServerOutboxConfigurationProvider.NamedServiceName);

        builder.SetOutboxProvider(SqlServerOutboxConfigurationProvider.NamedServiceName, (busName, providerConfig) =>
        {
            builder.Services.AddOptions<SqlServerOutboxOptions>(busName)
                .Bind(providerConfig)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        });
        return builder;
    }
}
