using myapi.core.App;
using myapi.Middleware;

IApp app = App.Init<Program>(5050)
    .AddMiddleware<ExampleMiddleware>();

app.Run();