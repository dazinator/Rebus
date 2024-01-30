namespace Tests;

using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Rebus.Extensions.Configuration;
using Rebus.Extensions.Configuration.FileSystem;
using Rebus.Extensions.Configuration.InMemory;
using Rebus.Extensions.Configuration.ServiceBus;
using Rebus.Extensions.Configuration.SqlServer;
using Rebus.Transport.InMem;

[Documentation]
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
        var services = new ServiceCollection().AddRebusFromConfiguration(configuration.GetSection("Rebus"), a => a.UseFileSystemTransportProvider()
                .UseInMemoryTransportProvider()
                .UseServiceBusTransportProvider()
                .UseSqlServerOutboxProvider());

        await Verify(services);
    }

    [Fact]
    public async Task InMemoryTransportOptions_JsonExample()
    {
        // Register a func that can resolve a network by name, you can then refer to this name in config.
        var services = new ServiceCollection().AddSingleton<Func<string, InMemNetwork>>((sp) => (name) =>
        {
            var network = new InMemNetwork();
            return network;
        });

        var options = new InMemoryRebusTransportOptions() { NetworkName = "Test", RegisterForSubscriptionStorage = true };
        var json = JsonSerializer.Serialize(options, new JsonSerializerOptions() { WriteIndented = true });
        await Verify(json);
    }

    [Fact]
    public async Task InMemoryTransportOptions_UsageExample()
    {
        // Register a func that can resolve a network by name, you can then refer to this name in config.
        var services = new ServiceCollection().AddSingleton<Func<string, InMemNetwork>>((sp) => (name) =>
        {
            var network = new InMemNetwork();
            return network;
        });

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddInMemoryCollection(new Dictionary<string, string>() { { "Rebus:Buses:Default:Transport:ProviderName", InMemoryRebusTransportConfigurationProvider.NamedServiceName }, })
            .Build();

        // Arrange
        services.AddRebusFromConfiguration(configuration.GetSection("Rebus"), a => a.UseInMemoryTransportProvider());

        await Verify(services);
    }

    [Fact]
    public async Task ServiceBusOptions_JsonExample()
    {
        var options = new ServiceBusRebusTransportOptions()
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
        var json = JsonSerializer.Serialize(options, new JsonSerializerOptions() { WriteIndented = true });
        await Verify(json);
    }

    [Fact]
    public async Task FileSystemOptions_JsonExample()
    {
        var options = new FileSystemRebusTransportOptions() { Prefetch = 10, BaseDirectory = "c://foo/bar", };
        var json = JsonSerializer.Serialize(options, new JsonSerializerOptions() { WriteIndented = true });
        await Verify(json);
    }

    [Fact]
    public async Task SqlOutbox_JsonExample()
    {
        var options = new SqlServerOutboxOptions() { ConnectionString = "Server=.;Database=Rebus;Trusted_Connection=True;", TableName = "Outbox" };
        var json = JsonSerializer.Serialize(options, new JsonSerializerOptions() { WriteIndented = true });
        await Verify(json);
    }
}
