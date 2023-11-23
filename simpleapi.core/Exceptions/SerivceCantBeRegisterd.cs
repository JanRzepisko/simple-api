namespace simpleapi.core.Exceptions;

public class ServiceCantBeRegistered : Exception
{
    public ServiceCantBeRegistered(string message) : base("Service Cant Be Registered " + message) {}
    public ServiceCantBeRegistered() : base("Service Cant Be Registered") {}
}