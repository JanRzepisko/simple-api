using simpleapi.core.App;
using simpleapi.Middleware;

IApp app = App.Init<Program>(5050)
    .AddMiddleware<ExampleMiddleware>();
app.Run();

