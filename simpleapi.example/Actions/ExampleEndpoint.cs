using System.Diagnostics;
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
    public class Handler : IEndpoint<int, object>
    {
        readonly IExampleService _exampleService;
        public Handler(IExampleService exampleService)
        {
            _exampleService = exampleService;
        }

        public async Task<object> Handle(int command)
        {
            int x = 3;
            return 
                new { res = 123 / x };
        }
    }
}
