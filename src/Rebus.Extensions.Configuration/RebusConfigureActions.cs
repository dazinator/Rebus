namespace Rebus.Extensions.Configuration;

using Config;

public class RebusConfigureActions
{

    private List<Func<RebusConfigurer, IServiceProvider, RebusConfigurer>> _callbacks = new List<Func<RebusConfigurer, IServiceProvider, RebusConfigurer>>();
    public void Add(Func<RebusConfigurer, IServiceProvider, RebusConfigurer> configurer)
    {
        _callbacks.Add(configurer);
    }

    internal void InvokeCallbacks(RebusConfigurer configurer, IServiceProvider serviceProvider)
    {
        foreach (var callback in _callbacks)
        {
            callback?.Invoke(configurer, serviceProvider);
        }
    }
}
