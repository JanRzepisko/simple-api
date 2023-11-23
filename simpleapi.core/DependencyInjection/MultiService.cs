namespace simpleapi.core.DependencyInjection;

public class MultiService
{
    public MultiService(Type? @interface, Type implementation)
    {
        Interface = @interface;
        Implementation = implementation;
    }

    internal Type? Interface { get; }
    internal Type Implementation { get; }

    public T CrateInstance<T>() where T : class
    {
        return (T)Activator.CreateInstance(Implementation)!;
    }
}