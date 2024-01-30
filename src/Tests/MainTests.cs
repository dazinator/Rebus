namespace Tests;

using Microsoft.Extensions.Configuration;
using Rebus.Extensions.Configuration;
using Rebus.Extensions.Configuration.FileSystem;
using Rebus.Extensions.Configuration.InMemory;
using Rebus.Extensions.Configuration.ServiceBus;
using Rebus.Extensions.Configuration.SqlServer;

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
    public async Task Usage_CanConfigureRebusFromConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        // Arrange
        var services = new ServiceCollection().AddRebusFromConfiguration(configuration.GetSection("Rebus"), a =>
        {
            a.UseFileSystemTransportProvider()
                .UseInMemoryTransportProvider()
                .UseServiceBusTransportProvider()
                .UseSqlServerOutboxProvider();
        });

        await Verify(services);
        //await using var sp = services.BuildServiceProvider();
        //sp.StartRebus();
    }
}
