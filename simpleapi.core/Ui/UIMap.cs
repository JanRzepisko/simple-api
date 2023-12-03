using System.Security;
using simpleapi.core.App;
using simpleapi.core.Attributes;
using simpleapi.core.Enums;
using simpleapi.core.Models;

namespace simpleapi.core.Ui;

public static class UiMap
{
    public static App.App AddUiMap(this App.App app, string uiMapPath = "uimap")
    {
        var body = app.CreateUiMap();
        var newCnf = new UiMapConfiguration()
        {
            Exist = true,
            Body = body,
            UiMapPath = uiMapPath
        };
        app.UiMapCnf = newCnf;
        return app;
    }

    private static string CreateUiMap(this App.App app)
    {
        return "<html><body>Cześć jestem janek</body></html>";
    }
}