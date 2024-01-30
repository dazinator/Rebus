namespace Rebus.Extensions.Configuration.InMemory;

using System.ComponentModel.DataAnnotations;

public class InMemoryRebusTransportOptions
{
    [Required] public string NetworkName { get; set; }

    public bool RegisterForSubscriptionStorage { get; set; } = true;
}
