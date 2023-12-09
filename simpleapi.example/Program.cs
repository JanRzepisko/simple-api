using simpleapi.core.App;
using simpleapi.core.DefaultFeatures;
using simpleapi.core.DependencyInjection;
using simpleapi.core.Middleware;
using simpleapi.core.Ui;
using simpleapi.example.Middleware;
using simpleapi.example.Services;

IApp app = App.Init<Program>(5050)
    .RegisterSingletonService<INoteService, NoteService>(new NoteService())
    .AddResponseWrapping()
    .AddUiMap();
app.Run();

