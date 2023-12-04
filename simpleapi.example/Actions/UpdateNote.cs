using System.Diagnostics;
using simpleapi.core.Attributes;
using simpleapi.core.Enums;
using simpleapi.core.Models;
using simpleapi.example.Services;

namespace simpleapi.example.Actions;

[Api("/update", Method.PUT)]
public static class UpdateNote
{
    public class Command
    {
        public int Id { get; set; }
        public string Tittle { get; set; }
        public string Text { get; set; }
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
            _noteService.Update(new Note()
            {
                Id = command.Id,
                Tittle = command.Tittle,
                Text = command.Text
            });
            return "Your note was updated! :)";
        }
    }
}
