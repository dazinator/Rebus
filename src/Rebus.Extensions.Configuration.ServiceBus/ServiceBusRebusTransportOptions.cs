namespace Rebus.Extensions.Configuration.ServiceBus;

using System.ComponentModel.DataAnnotations;

public class ServiceBusRebusTransportOptions
{
    [Required] public string ConnectionString { get; set; }

    public string ConnectionStringAccessKey { get; set; }

    /// <summary>
    ///     Enable partitioning.
    /// </summary>
    /// <remarks>
    ///     you'll get the exception if AutoDeleteOnIdle is set to any value (and EnablePartitioning is TRUE)
    ///     https://stackoverflow.com/questions/25563145/azure-service-bus-autodeleteonidle
    /// </remarks>
    public bool? EnablePartitioning { get; set; }

    public int? MessagePayloadSizeLimitInBytes { get; set; }

    /// <summary>
    ///     If specified, it must be at least 20 seconds and at most 1 day
    /// </summary>
    public TimeSpan? DuplicateDetectionHistoryTimeWindow { get; set; }

    /// <summary>
    ///     if specified it must be at least five minutes.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    ///     you'll get the exception if AutoDeleteOnIdle is set to any value (and EnablePartitioning is TRUE)
    ///     https://stackoverflow.com/questions/25563145/azure-service-bus-autodeleteonidle
    /// </remarks>
    public TimeSpan? AutoDeleteOnIdle { get; set; }

    /// <summary>
    ///     if specified it must be at least one second.
    /// </summary>
    /// <returns></returns>
    public TimeSpan? DefaultMessageTimeToLive { get; set; }

    /// <summary>
    ///     if specified, it must be at least 5 seconds and at most 5 minutes
    /// </summary>
    /// <returns></returns>
    public TimeSpan? MessagePeekLockDuration { get; set; }

    /// <summary>
    ///     if set, cannot be 0.
    /// </summary>
    public int? NumberOfMessagesToPrefetch { get; set; }


    public bool? AutomaticallyRenewPeekLock { get; set; }

    public bool? UseLegacyNaming { get; set; }

    public bool AutoCreateQueues { get; set; } = true;

    public bool CheckQueueConfiguration { get; set; } = true;

    public TimeSpan? ReceiveOperationTimeout { get; set; }

    public string GetFormattedConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(ConnectionStringAccessKey))
        {
            return string.Format(ConnectionString, ConnectionStringAccessKey);
        }

        return ConnectionString;
    }
}
