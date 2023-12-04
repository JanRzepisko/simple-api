using System.Diagnostics;
using simpleapi.core.Attributes;
using simpleapi.core.Enums;
using simpleapi.core.Models;
using simpleapi.example.Services;

namespace simpleapi.example.Actions;

[Api("/getById", Method.GET)]
public static class GetNoteById
{
    public class Command
    {
        public int id { get; set; }
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
            return _noteService.Get(command.id);
        }
    }
}
