namespace Rebus.Extensions.Configuration.SqlServer;

using Microsoft.Extensions.Options;

public class ConfigureSqlServerOutboxConfigProviderOptions : IPostConfigureOptions<BusOptions>
{
    private readonly IOptionsMonitor<SqlServerOutboxOptions> _optionsMonitor;

    public ConfigureSqlServerOutboxConfigProviderOptions(IOptionsMonitor<SqlServerOutboxOptions> optionsMonitor) => _optionsMonitor = optionsMonitor;

    public void PostConfigure(string name, BusOptions options)
    {
        if (options.Outbox?.ProviderName == SqlServerOutboxConfigurationProvider.NamedServiceName)
        {
            options.OutboxConfigurationProvider = new SqlServerOutboxConfigurationProvider(_optionsMonitor);
        }
    }
}
