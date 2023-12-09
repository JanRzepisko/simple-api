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
    public class Handler : IEndpoint<Command, Note>
    {
        private readonly INoteService _noteService;
        public Handler(INoteService noteService)
        {
            _noteService = noteService;
        }
        public Task<Note> Handle(Command command)
        {
            var note = new Note()
            {
                Tittle = command.Tittle,
                Text = command.Text
            }
            ;
            _noteService.Add(note);
            return Task.FromResult(note);
        }

        [ExampleEndpointCommand] public static Command Example = new Command()
        {
            Tittle = "Siema jestem Janek",
            Text = "Janek tu był, janek tu dalej jest"
        };
    }
}
