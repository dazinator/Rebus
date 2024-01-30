namespace Core;

public class RebusOptions
{
    public string DefaultBus { get; set; }

    public Dictionary<string, BusOptions> Buses { get; set; } = new();

    public List<string> InMemoryNetworks { get; set; } = new();

    public List<string> InMemoryDataStores { get; set; } = new();

    //  public TransportOptions Transport { get; set; }
}
