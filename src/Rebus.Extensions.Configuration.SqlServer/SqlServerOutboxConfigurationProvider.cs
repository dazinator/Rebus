namespace Rebus.Extensions.Configuration.SqlServer;

using Config;
using Config.Outbox;
using Microsoft.Extensions.Options;

public class SqlServerOutboxConfigurationProvider : RebusOutboxConfigurationProvider
{
    public const string NamedServiceName = "SqlServer";
    private readonly IOptionsMonitor<SqlServerOutboxOptions> _options;


    public SqlServerOutboxConfigurationProvider(IOptionsMonitor<SqlServerOutboxOptions> options) => _options = options;

    public override void ConfigureOutbox(string busName, BusOptions busOptions, RebusConfigurer configurer)
    {
        var options = string.IsNullOrWhiteSpace(busName) ? _options.CurrentValue : _options.Get(busName);
        var sqlServerOptions = _options.CurrentValue;
        configurer = configurer.Outbox(a => a.StoreInSqlServer(options.ConnectionString, options.TableName));
    }
}
