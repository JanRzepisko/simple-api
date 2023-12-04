namespace simpleapi.example.Services;

public class NoteService : INoteService
{
    private List<Note> _notes { get; } = new();
    public void Add(Note note)
    {
        if(_notes.Count == 0)
            note.Id = 1;
        else
            note.Id = _notes.MaxBy(c => c.Id).Id + 1;
        _notes.Add(note);
    }

    public void Remove(int id)
    {
        _notes.Where(c => c.Id == id).ToList().ForEach(c => _notes.Remove(c));
    }

    public void Update(Note note)
    {
        _notes.Where(c => c.Id == note.Id).ToList().ForEach(c =>
        {
            c.Tittle = note.Tittle;
            c.Text = note.Text;
        });
    }

    public Note Get(int id)
    {
        return _notes.FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Note> GetAll()
    {
        return _notes;
    }
}