namespace Rebus.Extensions.Configuration;

public class BusOptions
{
    public string? QueuePrefix { get; set; }
    public TransportOptions Transport { get; set; }

    public OutboxOptions Outbox { get; set; }
    internal bool IsDefault { get; set; }

    public RebusTransportConfigurationProvider? TransportConfigurationProvider { get; set; }


    public RebusOutboxConfigurationProvider? OutboxConfigurationProvider { get; set; }

    public string GetPrefixedQueueName(string name)
    {
        if (string.IsNullOrWhiteSpace(QueuePrefix))
        {
            return name;
        }

        return $"{QueuePrefix}{name}";
    }

    public string GetInputQueueName()
    {
        if (string.IsNullOrWhiteSpace(QueuePrefix))
        {
            return Transport.QueueName;
        }

        return $"{QueuePrefix}{Transport.QueueName}";
    }
}
