namespace simpleapi.core.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base("Not Found" + message) {}
    public NotFoundException() : base("Not Found") {}

}