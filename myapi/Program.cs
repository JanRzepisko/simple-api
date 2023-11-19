using myapi.core.App;

IApp app = App.Init<Program>(5050);
app.Run();

Console.Read();