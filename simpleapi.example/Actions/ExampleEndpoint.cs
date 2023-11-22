using simpleapi.core.Attributes;
using simpleapi.core.Enums;
using simpleapi.core.Models;
using simpleapi.example.Services;

namespace simpleapi.example.Actions;

[Api("/test1", Method.GET)]
public static class ExampleEndpoint
{
    public class Command
    {
    }
    public class Handler : IEndpoint<Command, string>
    {
        string Test;
        IExampleService ExampleService;
        public Handler(string test, IExampleService exampleService)
        {
            Test = test;
            ExampleService = exampleService;
        }
        
        public async Task<string> Handle(Command command)
        {
            return $"{Test} {ExampleService.TwoPlusTwo()}";
        }
    }
}
