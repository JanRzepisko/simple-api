using System.Diagnostics;
using simpleapi.core.Attributes;
using simpleapi.core.Enums;
using simpleapi.core.Models;
using simpleapi.example.Services;

namespace simpleapi.example.Actions;

[Api("/all", Method.GET)]
public static class GetAllNotes
{
    public class Command
    {
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
            return _noteService.GetAll();
        }
    }
}
