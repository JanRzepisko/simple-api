using simpleapi.core.Attributes;
using simpleapi.core.Enums;
using simpleapi.core.Models;
using simpleapi.example.Services;

namespace simpleapi.example.Actions;

[Api("/create", Method.POST)]
public static class CreateNote
{
    public class Command
    {
        public string Tittle { get; set; }
        public string Text { get; set; }
    }
    public class Handler : IEndpoint<Command, string>
    {
        private readonly INoteService _noteService;
        public Handler(INoteService noteService)
        {
            _noteService = noteService;
        }
        public Task<string> Handle(Command command)
        {
            _noteService.Add(new Note()
            {
                Tittle = command.Tittle,
                Text = command.Text
            });
            return Task.FromResult("Your note was created! :)");
        }
    }
}
