﻿using simpleapi.core.App;
using simpleapi.core.Middleware;
using simpleapi.example.Middleware;

IApp app = App.Init<Program>(5050)
    .AddMiddleware<ExampleMiddleware>();
app.Run();

