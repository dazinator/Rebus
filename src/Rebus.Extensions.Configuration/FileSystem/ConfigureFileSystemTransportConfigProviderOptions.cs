namespace Rebus.Extensions.Configuration.FileSystem;

using Microsoft.Extensions.Options;

public class ConfigureFileSystemTransportConfigProviderOptions : IPostConfigureOptions<BusOptions>
{
    private readonly IOptionsMonitor<FileSystemRebusTransportOptions> _optionsMonitor;

    public ConfigureFileSystemTransportConfigProviderOptions(IOptionsMonitor<FileSystemRebusTransportOptions> optionsMonitor) => _optionsMonitor = optionsMonitor;

    public void PostConfigure(string name, BusOptions options)
    {
        if (options.Transport?.ProviderName == FileSystemRebusTransportConfigurationProvider.NamedServiceName)
        {
            options.TransportConfigurationProvider = new FileSystemRebusTransportConfigurationProvider(_optionsMonitor);
        }
    }
}
