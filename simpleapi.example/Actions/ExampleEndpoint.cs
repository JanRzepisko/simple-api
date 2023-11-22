using simpleapi.core.Attributes;
using simpleapi.core.Enums;
using simpleapi.core.Models;

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
        public Handler(string test)
        {
            Test = test;
        }
        
        public async Task<string> Handle(Command command)
        {
            return Test;
        }
    }
}
