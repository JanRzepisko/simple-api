using simpleapi.core.App;
using simpleapi.core.DependencyInjection;
using simpleapi.core.Middleware;
using simpleapi.example.Middleware;

string test = "test";

IApp app = App.Init<Program>(5050)
    .AddMiddleware<ExampleMiddleware>()
    .RegisterSingletonService(test);
app.Run();

