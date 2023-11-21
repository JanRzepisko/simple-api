using simpleapi.core.Attributes;
using simpleapi.core.Enums;
using simpleapi.core.Models;

namespace simpleapi.example.Actions;

[Api("/test2", Method.POST)]
public static class ExampleEndpoint2
{
    public class Command
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
    }
    public class Handler : IEndpoint<Command, int>
    {
        public async Task<int> Handle(Command command)
        {
            return command.A + command.B + command.C;
        }
    }
}
