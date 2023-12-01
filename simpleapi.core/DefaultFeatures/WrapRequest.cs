using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace simpleapi.core.DefaultFeatures;

public static class WrapRequest
{
    public static App.App AddResponseWrapping(this App.App app)
    {
        app.WrapResponse = true;
        return app;
    }

    internal static string PrepareResponse(this App.App app, object? res, int statusCode)
    {
        if (!app.WrapResponse)
            return JsonConvert.SerializeObject(res, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

        if (statusCode is >= 200 and < 300)
        {
            var response = new WrapModel()
            {
                Data = res,
                Errors = null,
                StatusCode = statusCode
            };
            return JsonConvert.SerializeObject(response, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
        else
        {
            var response = new WrapModel()
            {
                Data = null,
                Errors = res,
                StatusCode = statusCode
            };
            return JsonConvert.SerializeObject(response, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}