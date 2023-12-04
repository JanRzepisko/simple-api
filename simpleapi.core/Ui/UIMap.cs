using System.Security;
using Newtonsoft.Json;
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
        var body = File.ReadAllText("Ui/uimap.html");
        var component = File.ReadAllText("Ui/MapComponent");
        var jsonInputComponent = File.ReadAllText("Ui/JsonInput");
        var paramsInputComponent = File.ReadAllText("Ui/ParamsComponent");
        var components = new List<string>();

        foreach (var item in app._endpoints)
        {
            var componentItem = component;
            if (item.Method != Method.GET && item.Method != Method.HEAD)
            {
                componentItem = componentItem.Replace("{DATA-INPUT}", jsonInputComponent);
                componentItem = componentItem.Replace("{INPUT}", JsonConvert.SerializeObject(item.Command));
            }
            else
            {
                string startTable = "<table id='{ENDPOINT-NAME}-table'>".Replace("{ENDPOINT-NAME}", item.Handler.DeclaringType.Name);
                var paramsArray = new List<string>();
                var props =item.Command.GetType().GetProperties();
                foreach (var param in props)
                {
                    var paramItem = paramsInputComponent;
                    paramItem = paramItem.Replace("{PARAMETER-NAME}", param.Name);
                    paramsArray.Add(paramItem);
                }
                componentItem = componentItem.Replace("{DATA-INPUT}", startTable + string.Join("", paramsArray) + "</table>");
            }
            
            componentItem = componentItem.Replace("{METHOD}", item.Method.ToString());
            componentItem = componentItem.Replace("{ENDPOINT-PATH}", item.Path);
            componentItem = componentItem.Replace("{ENDPOINT-NAME}", item.Handler.DeclaringType.Name);
            
            components.Add(componentItem);
        }
        
        body = body.Replace("{BODY}", string.Join("", components));
        return body;
    }
}
