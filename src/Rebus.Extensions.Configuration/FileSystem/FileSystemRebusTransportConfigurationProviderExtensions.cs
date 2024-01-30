﻿namespace Rebus.Extensions.Configuration.FileSystem;

using Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class FileSystemRebusTransportConfigurationProviderExtensions
{
    public static ConfigurationProvidersRegistrationBuilder UseFileSystemTransportProvider(this ConfigurationProvidersRegistrationBuilder builder)
    {
        builder.Services.AddSingleton<IPostConfigureOptions<BusOptions>, ConfigureFileSystemTransportConfigProviderOptions>();
        builder.SetProviderConfigureHook(FileSystemRebusTransportConfigurationProvider.NamedServiceName, ProviderSectionTypeNames.Transport, (busName, transportConfig) => builder.Services.AddOptions<FileSystemRebusTransportOptions>(busName)
            .Bind(transportConfig)
            .ValidateDataAnnotations()
            .ValidateOnStart());
        return builder;
    }
}
