using myapi.core.App;

IApp app = new App().Init<Program>(5050);
app.Run();

Console.Read();