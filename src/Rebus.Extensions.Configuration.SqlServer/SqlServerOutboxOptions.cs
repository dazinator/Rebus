namespace Rebus.Extensions.Configuration.SqlServer;

using System.ComponentModel.DataAnnotations;

public class SqlServerOutboxOptions
{
    [Required] public string ConnectionString { get; set; } = string.Empty;

    [Required] public string TableName { get; set; } = "Outbox";
}
