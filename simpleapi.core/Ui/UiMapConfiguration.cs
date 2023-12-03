namespace simpleapi.core.Ui;

public class UiMapConfiguration
{
    public bool Exist { get; set; }
    public string? UiMapPath { get; set; }
    public string? Body { get; set; }

    public UiMapConfiguration()
    {
        Exist = false;
        UiMapPath = "";
    }
}