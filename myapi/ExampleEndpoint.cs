using myapi.core.Attributes;
using myapi.core.Enums;
using myapi.core.Models;

namespace myapi;

[Api("/abc/aaa", Method.GET)]
public static class ExampleEndpoint
{
    [Command]
    public class Command
    {
        public double A { get; set; }
        public double B { get; set; }
    }
    public class Handler : IEndpoint<Command, object>
    {
        public async Task<object> Handle(Command command)
        {
            return new
            {
                res = command.A + command.B
            };
        }
    }
}
