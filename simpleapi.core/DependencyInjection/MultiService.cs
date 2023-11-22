namespace simpleapi.core.DependencyInjection;

public class MultiService
{
    public MultiService(Type @interface, Type implementation)
    {
        Interface = @interface;
        Implementation = implementation;
    }

    internal Type Interface { get; }
    internal Type Implementation { get; }
    
    public object CrateInstance()
    {
        return Convert.ChangeType(Activator.CreateInstance(Implementation), Interface);
    }
}