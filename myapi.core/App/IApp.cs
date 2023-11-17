namespace myapi.core.App;

public interface IApp
{
    public App Init<TEntryPoint>(int port);
    public Task Run();
}