namespace Rebus.Extensions.Configuration.FileSystem;

using Microsoft.Extensions.DependencyInjection;

public static class FileSystemRebusTransportConfigurationProviderExtensions
{
    public static RebusConfigurationProviderOptionsBuilder UseFileSystemTransportProvider(this RebusConfigurationProviderOptionsBuilder builder)
    {
        builder.AddTransportConfigurationProvider<FileSystemRebusTransportConfigurationProvider>(FileSystemRebusTransportConfigurationProvider.NamedServiceName);

        builder.SetTransportProvider(FileSystemRebusTransportConfigurationProvider.NamedServiceName, (busName, transportConfig) => builder.Services.AddOptions<FileSystemRebusTransportOptions>(busName)
                .Bind(transportConfig)
                .ValidateDataAnnotations()
                .ValidateOnStart());
        return builder;
    }
}
