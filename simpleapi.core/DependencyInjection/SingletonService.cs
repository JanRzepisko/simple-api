namespace simpleapi.core.DependencyInjection;

public class SingletonService
{
    public SingletonService(object instance)
    {
        Instance = instance;
    }

    internal object Instance { get; }
}