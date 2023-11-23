namespace simpleapi.core.DependencyInjection;

public class SingletonService
{
    public SingletonService(object instance)
    {
        Instance = instance;
        Interface = null;
    }
    
    public SingletonService(object instance, Type @interface)
    {
        Instance = instance;
        Interface = @interface;
    }

    internal object Instance { get; }
    internal Type Interface { get;  }
}