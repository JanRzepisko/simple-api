using System.Diagnostics;
using simpleapi.core.Attributes;
using simpleapi.core.Enums;
using simpleapi.core.Models;
using simpleapi.example.Services;

namespace simpleapi.example.Actions;

[Api("/remove", Method.DELETE)]
public static class RemoveNote
{
    public class Command
    {
        public int Id { get; set; }
    }
    public class Handler : IEndpoint<Command, object>
    {
        private readonly INoteService _noteService;
        public Handler(INoteService noteService)
        {
            _noteService = noteService;
        }
        public async Task<object> Handle(Command command)
        {
            _noteService.Remove(command.Id);
            return "Your note was removed! :(";
        }
    }
}
