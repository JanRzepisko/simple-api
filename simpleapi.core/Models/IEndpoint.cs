namespace simpleapi.core.Models;

public interface IEndpoint<In, Out> : IEndpointModel
{
    Task<int> Handle(In command);
}