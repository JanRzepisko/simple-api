namespace simpleapi.core.Exceptions;

public class DependencyInjectionException : Exception
{
    public DependencyInjectionException(string message) : base("On constructing handler in " + message ) {}
}