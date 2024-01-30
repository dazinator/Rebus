namespace Rebus.Extensions.Configuration.FileSystem;

using Config;
using Core;
using Core.Infrastructure.Rebus;
using Microsoft.Extensions.Options;
using Transport;
using Transport.FileSystem;

public class FileSystemRebusTransportConfigurationProvider : RebusTransportConfigurationProvider
{
    public const string NamedServiceName = "FileSystem";
    private readonly IOptionsMonitor<FileSystemRebusTransportOptions> _options;


    public FileSystemRebusTransportConfigurationProvider(IOptionsMonitor<FileSystemRebusTransportOptions> options) => _options = options;


    public override void ConfigureTransport(
        string busName,
        BusOptions busOptions,
        StandardConfigurer<ITransport> configurer
    )
    {
        var options = string.IsNullOrWhiteSpace(busName) ? _options.CurrentValue : _options.Get(busName);
        var transportOptions = busOptions.Transport;
        var builder = configurer.UseFileSystem(options.GetBaseDirectoryExpanded(), busOptions.GetInputQueueName());

        if (options.Prefetch != null)
        {
            builder = builder.Prefetch(options.Prefetch.Value);
        }
    }
}
