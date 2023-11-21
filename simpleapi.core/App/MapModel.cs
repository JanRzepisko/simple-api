using simpleapi.core.Enums;

namespace simpleapi.core.App;

public class MapModel
{
    public object? Handler { get; set; }
    public object Command { get; set; }
    public Type OutputType { get; set; }
    public Method Method { get; set; }
    public string? Path { get; set; }
}