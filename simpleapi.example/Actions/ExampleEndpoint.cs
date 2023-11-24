using simpleapi.core.Attributes;
using simpleapi.core.Enums;
using simpleapi.core.Models;
using simpleapi.example.Services;

namespace simpleapi.example.Actions;

[Api("/example", Method.GET)]
public static class ExampleEndpoint
{
    public class Command
    {
        
    }
    public class Handler : IEndpoint<Command, int>
    {
        readonly IExampleService _exampleService;
        public Handler(IExampleService exampleService)
        {
            _exampleService = exampleService;
        }
        
        public Task<int> Handle(Command command)
        {
            return Task.FromResult(_exampleService.AddTwo()) ;
        }
    }
}
