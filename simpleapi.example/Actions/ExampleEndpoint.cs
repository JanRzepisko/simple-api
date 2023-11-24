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
    public class Handler : IEndpoint<int, string>
    {
        readonly IExampleService _exampleService;
        public Handler(IExampleService exampleService)
        {
            _exampleService = exampleService;
        }

        public async Task<string> Handle(int command)
        {
            return "działa";
        }
    }
}
