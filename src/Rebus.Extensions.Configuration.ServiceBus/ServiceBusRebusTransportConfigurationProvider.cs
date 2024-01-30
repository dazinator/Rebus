namespace Rebus.Extensions.Configuration.ServiceBus;

using Config;
using Core;
using Core.Infrastructure.Rebus;
using Microsoft.Extensions.Options;
using Transport;

public class ServiceBusRebusTransportConfigurationProvider : RebusTransportConfigurationProvider
{
    public const string NamedServiceName = "ServiceBus";
    private readonly IOptionsMonitor<ServiceBusRebusTransportOptions> _options;


    public ServiceBusRebusTransportConfigurationProvider(IOptionsMonitor<ServiceBusRebusTransportOptions> options) => _options = options;


    public override void ConfigureTransport(
        string busName,
        BusOptions busOptions,
        StandardConfigurer<ITransport> configurer
    )
    {
        var options = string.IsNullOrWhiteSpace(busName) ? _options.CurrentValue : _options.Get(busName);
        var transportOptions = busOptions.Transport;
        var connString = options.GetFormattedConnectionString();
        var builder = configurer.UseAzureServiceBus(connString, busOptions.GetInputQueueName());

        if (options.UseLegacyNaming ?? false)
        {
            builder = builder.UseLegacyNaming();
        }

        if (options.ReceiveOperationTimeout != null)
        {
            builder = builder.SetReceiveOperationTimeout(options.ReceiveOperationTimeout.Value);
        }

        if (options.DefaultMessageTimeToLive != null)
        {
            builder = builder.SetDefaultMessageTimeToLive(options.DefaultMessageTimeToLive.Value);
        }

        if (options.NumberOfMessagesToPrefetch != null)
        {
            builder = builder.EnablePrefetching(options.NumberOfMessagesToPrefetch.Value);
        }

        if (options.EnablePartitioning ?? false)
        {
            builder = builder.EnablePartitioning();
        }

        if (!options.AutoCreateQueues)
        {
            builder = builder.DoNotCreateQueues();
        }

        if (!options.CheckQueueConfiguration)
        {
            builder = builder.DoNotCheckQueueConfiguration();
        }

        if (options.AutomaticallyRenewPeekLock ?? false)
        {
            builder = builder.AutomaticallyRenewPeekLock();
        }

        if (options.MessagePeekLockDuration != null)
        {
            builder = builder.SetMessagePeekLockDuration(options.MessagePeekLockDuration.Value);
        }

        if (options.DuplicateDetectionHistoryTimeWindow != null)
        {
            builder = builder.SetDuplicateDetectionHistoryTimeWindow(options.DuplicateDetectionHistoryTimeWindow.Value);
        }

        if (options.AutoDeleteOnIdle != null)
        {
            builder = builder.SetAutoDeleteOnIdle(options.AutoDeleteOnIdle.Value);
        }

        if (options.MessagePayloadSizeLimitInBytes != null)
        {
            builder = builder.SetMessagePayloadSizeLimit(options.MessagePayloadSizeLimitInBytes.Value);
        }
    }
}
