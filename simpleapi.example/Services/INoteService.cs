namespace simpleapi.example.Services;

public interface INoteService
{
    void Add(Note note);
    void Remove(int id);
    void Update(Note note);
    Note Get(int id);
    IEnumerable<Note> GetAll();
}