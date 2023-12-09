namespace simpleapi.core.Models;

public interface IEndpoint<In, Out> : IEndpointModel
{
    Task<Out> Handle(In command);
}
public interface IEndpoint<Out> : IEndpointModel
{
    Task<Out> Handle();
}