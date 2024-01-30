﻿namespace Rebus.Extensions.Configuration.SqlServer;

using Core;
using Microsoft.Extensions.Options;

public class ConfigureSqlServerOutboxConfigProviderOptions: IPostConfigureOptions<BusOptions>
{
    private readonly IOptionsMonitor<SqlServerOutboxOptions> _optionsMonitor;

    public ConfigureSqlServerOutboxConfigProviderOptions(IOptionsMonitor<SqlServerOutboxOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public void PostConfigure(string name, BusOptions options)
    {
        if (options.Transport?.ProviderName == SqlServerOutboxConfigurationProvider.NamedServiceName)
        {
            options.OutboxConfigurationProvider = new SqlServerOutboxConfigurationProvider(_optionsMonitor);
        }
    }
}
