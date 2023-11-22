namespace simpleapi.core.DependencyInjection;

public class SingletonService<T>
{
    public SingletonService(T instance)
    {
        Instance = instance;
    }

    private T Instance { get; }
}