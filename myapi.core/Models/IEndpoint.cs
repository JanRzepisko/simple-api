namespace myapi.core.Models;

public interface IEndpoint<In, Out> : IEndpointModel
{
    Task<Out> Handle(In command);
}