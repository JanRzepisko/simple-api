namespace simpleapi.core.Exceptions;

public class HandlerCannotBeRegister : Exception
{
    public HandlerCannotBeRegister(string message) : base($"Handler {message} Cannot be register") {}
    public HandlerCannotBeRegister() : base("Handler Cannot be register") {}
}