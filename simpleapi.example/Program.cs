using simpleapi.core.App;
using simpleapi.core.DependencyInjection;
using simpleapi.example.Services;

IApp app = App.Init<Program>(5050)
    .RegisterMultiService<IExampleService, ExampleService>()
    .RegisterSingletonService(new ExampleService());
app.Run();

