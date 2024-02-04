namespace Rebus.Extensions.Configuration.FileSystem;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class FileSystemRebusTransportConfigurationProviderExtensions
{
    public static ConfigurationProvidersRegistrationBuilder FileSystemTransport(this ConfigurationProvidersRegistrationBuilder builder)
    {
        AddFileSystemTransportConfigureOptions(builder);
        builder.SetProviderConfigureHook(FileSystemRebusTransportConfigurationProvider.NamedServiceName, ProviderSectionTypeNames.Transport, (busName, transportConfig) => builder.Services.AddOptions<FileSystemRebusTransportOptions>(busName)
            .Bind(transportConfig)
            .ValidateDataAnnotations()
            .ValidateOnStart());
        return builder;
    }

    private static ConfigurationProvidersRegistrationBuilder AddFileSystemTransportConfigureOptions(this ConfigurationProvidersRegistrationBuilder builder)
    {
        builder.Services.AddSingleton<IPostConfigureOptions<BusOptions>, ConfigureFileSystemTransportConfigProviderOptions>();
        return builder;
    }

}
