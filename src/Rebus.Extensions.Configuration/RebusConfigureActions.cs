namespace Rebus.Extensions.Configuration;

using Config;

public class RebusConfigureActions
{

    private List<Func<BusConfigurationContext, RebusConfigurer>> _callbacks = new List<Func<BusConfigurationContext, RebusConfigurer>>();
    public void Add(Func<BusConfigurationContext, RebusConfigurer> configurer)
    {
        _callbacks.Add(configurer);
    }

    internal void InvokeCallbacks(BusConfigurationContext context)
    {
        foreach (var callback in _callbacks)
        {
            callback?.Invoke(context);
        }
    }
}

public class BusConfigurationContext
{

    public IServiceProvider ServiceProvider { get; set; }
    /// <summary>
    /// The rebus configurer being used to configure the bus. You can perform custom rebus configuration with this using rebus native api.
    /// </summary>
    public RebusConfigurer Configurer { get; set; }
    // /// <summary>
    // /// Any options that have been configured and applied already to the rebus configurer. Changing these is not recommended as changes here will not be applied.
    // /// </summary>
    // public BusOptions ConfiguredOptions { get; set; }
    /// <summary>
    /// The name of the rebus bus being configured.
    /// </summary>
    public string Name { get; set; }
}
