namespace Tests;

using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Rebus.Config;
using Rebus.Extensions.Configuration;
using Rebus.Extensions.Configuration.FileSystem;
using Rebus.Extensions.Configuration.InMemory;
using Rebus.Extensions.Configuration.ServiceBus;
using Rebus.Extensions.Configuration.SqlServer;
using Rebus.Persistence.FileSystem;
using Rebus.Transport.InMem;

[UnitTest]
[UsesVerify]
public class MainTests
{
    public MainTests(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
        DefaultServices = new ServiceCollection()
            .AddLogging(builder => builder.AddXUnit(OutputHelper));
    }

    public IServiceCollection DefaultServices { get; set; }

    public ITestOutputHelper OutputHelper { get; }

    [Fact]
    public async Task AddRebusFromConfiguration_ServicesAreRegistered()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        // Arrange
        var services = new ServiceCollection()
            .AddRebusFromConfiguration(configuration.GetSection("Rebus"), a =>
                a.UseFileSystemTransportProvider()
                    .UseInMemoryTransportProvider(networkName =>
                    {
                        return new InMemNetwork();
                    })
                    .UseServiceBusTransportProvider()
                    .UseSqlServerOutboxProvider());

        await Verify(services);
    }

    [Documentation]
    [Fact]
    public async Task InMemoryTransportOptions_JsonExample()
    {
        var options = new InMemoryRebusTransportOptions { NetworkName = "Test", RegisterForSubscriptionStorage = true };
        var json = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
        await Verify(json);
    }

    [Documentation]
    [Fact]
    public async Task InMemoryTransportOptions_UsageExample()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddInMemoryCollection(new Dictionary<string, string> { { "Rebus:Buses:Default:Transport:ProviderName", InMemoryRebusTransportConfigurationProvider.NamedServiceName } })
            .Build();

        // Arrange
        var services = new ServiceCollection()
            .AddRebusFromConfiguration(configuration.GetSection("Rebus"),
                a => a.UseInMemoryTransportProvider(networkName =>
                {
                    return new InMemNetwork();
                }));

        await Verify(services);
    }

    [Documentation]
    [Fact]
    public async Task ServiceBusOptions_JsonExample()
    {
        var options = new ServiceBusRebusTransportOptions
        {
            ConnectionString = "sb://foo.bar",
            EnablePartitioning = false,
            AutoCreateQueues = true,
            CheckQueueConfiguration = true,
            AutoDeleteOnIdle = TimeSpan.FromMinutes(60),
            ConnectionStringAccessKey = "hadwa",
            ReceiveOperationTimeout = TimeSpan.FromMinutes(1),
            UseLegacyNaming = false,
            DefaultMessageTimeToLive = TimeSpan.FromSeconds(60),
            MessagePeekLockDuration = TimeSpan.FromMinutes(2),
            AutomaticallyRenewPeekLock = true,
            DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10),
            NumberOfMessagesToPrefetch = 1,
            MessagePayloadSizeLimitInBytes = 50000000
        };
        var json = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
        await Verify(json);
    }

    [Documentation]
    [Fact]
    public async Task FileSystemOptions_JsonExample()
    {
        var options = new FileSystemRebusTransportOptions { Prefetch = 10, BaseDirectory = "c://foo/bar" };
        var json = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
        await Verify(json);
    }

    [Documentation]
    [Fact]
    public async Task SqlOutbox_JsonExample()
    {
        var options = new SqlServerOutboxOptions { ConnectionString = "Server=.;Database=Rebus;Trusted_Connection=True;", TableName = "Outbox" };
        var json = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
        await Verify(json);
    }


    [Theory]
    [InlineData(InMemoryRebusTransportConfigurationProvider.NamedServiceName)]
    public async Task TransportConfigurationProvider_IsConfigured(string transportProvider)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddInMemoryCollection(new Dictionary<string, string> { { "Rebus:Buses:Default:Transport:ProviderName", transportProvider } })
            .Build();

        // Arrange
        var services = new ServiceCollection()
            .AddRebusFromConfiguration(configuration.GetSection("Rebus"), a =>
                a.UseFileSystemTransportProvider()
                    .UseInMemoryTransportProvider(networkName =>
                    {
                        return new InMemNetwork();
                    })
                    .UseServiceBusTransportProvider());


        var sp = services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptionsMonitor<BusOptions>>();
        var busOptions = options.Get("Default");

        busOptions.TransportConfigurationProvider.ShouldNotBeNull();
    }

    [Fact]
    public void Can_Configure_Using_Callback()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();


        bool globalCallbackCalled = false;
        bool busSpecificCallbackCalled = false;

        // Arrange
        var services = new ServiceCollection()
            .AddRebusFromConfiguration(configuration.GetSection("Rebus"), a =>
                a.UseFileSystemTransportProvider()
                    .UseConfigureCallback((configure, sp) =>
                    {
                        globalCallbackCalled = true;
                        configure.Sagas(b => b.UseFilesystem("./foo"));
                        return configure;
                    })
                    .UseConfigureCallback("Default", (configure, sp) =>
                    {
                        busSpecificCallbackCalled = true;
                        return configure;
                    }));

        var sp = services.BuildServiceProvider();

        sp.StartRebus();
        globalCallbackCalled.ShouldBeTrue();
        busSpecificCallbackCalled.ShouldBeTrue();
    }
}
