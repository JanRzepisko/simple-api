﻿using System.Net;
using System.Reflection;
using System.Text;
using myapi.core.Attributes;
using myapi.core.Enums;
using myapi.core.Extensions;
using myapi.core.Models;
using Newtonsoft.Json;

namespace myapi.core.App;

public class App : IApp
{
    private HttpListener _server;
    private List<MapModel> Endpoints = new();
    private Assembly EntryPoint;
    private MethodInfo MethodToResolve;
    private int Port;
    private bool Inited = false;
    private bool Started = false;
    
    public Task Run()
    {
        if (!Inited)
            throw new Exception("App must be inited");
        _server = new HttpListener();
        _server.Prefixes.Add($"http://localhost:{Port}/");

        _server.Start();
        Console.WriteLine($"Listening on port {Port}...");

        Started = true;
        while (Started)
        {
            OnRequest();
        }

        return Task.CompletedTask;
    }
    public App Init<TEntryPoint>(int port)
    {
        Port = port;   
        MethodToResolve = typeof(App).GetMethods().FirstOrDefault(c => c.Name == "ResolveTask")!;
        EntryPoint = typeof(TEntryPoint).Assembly;
        var types = typeof(TEntryPoint).Assembly.GetTypes();
        var endpoints = types.Where(c => c.CustomAttributes
            .Any(attributeData =>attributeData.AttributeType == typeof(ApiAttribute))).ToList();

        foreach (var e in endpoints)
        {
            var command = e.GetMembers().FirstOrDefault(c => 
                c.CustomAttributes.Any(x => x.AttributeType == typeof(CommandAttribute)));

            var assembly = typeof(TEntryPoint).Assembly;
            var handlerType = ClassHierarchyExplorer.GetClassesImplementingInterface(typeof(IEndpointModel), assembly)
                .FirstOrDefault(c => 
                    c.GetMethods().Any(x => x.GetParameters().Any(x => x.ParameterType == command)));

            if (handlerType is null)
                throw new Exception($"Handler {handlerType!.Name} Cannot be registered");

            var baseOfEndpoint = e.CustomAttributes.FirstOrDefault();
            
            if(baseOfEndpoint is null)
                throw new Exception($"Handler {handlerType!.Name} Cannot be registered");
            
            var path = baseOfEndpoint.ConstructorArguments[0].Value!.ToString();
            var method =(Method)e.CustomAttributes.FirstOrDefault()!.ConstructorArguments[1].Value! ;
            
            
            if (command is null || method != Method.GET)
            {
                continue;
            }

            var paramsFromMethod = new object{};
            
            if (method == Method.GET)
            {
                //TODO
                //paramsFromMethod = handlerType.GetMethods()[0].GetParameters();
            }
            
            var map = new MapModel()
            {
                Handler = this.EntryPoint.CreateInstance(handlerType.FullName!),
                Command = command ?? paramsFromMethod ,
                Method = method,
                Path = path,
                OutputType = handlerType.GetMethods()[0].ReturnType.GetGenericArguments()[0]
            };
            var res = map.Handler!.GetType();
            var parentClass = res.DeclaringType;
            map.Command = Activator.CreateInstance(parentClass!.GetNestedTypes()[0]);
            Endpoints.Add(map);
        }

        Inited = true;
        return this;
    }
    private async Task OnRequest()
    {
        var ctx = _server.GetContext();
        var body = new StreamReader(ctx.Request.InputStream).ReadToEndAsync();
        var endpoint = Endpoints.FirstOrDefault(c => c.Path == ctx.Request.RawUrl.Split("?")[0]);
        if (endpoint is null)
            Return404(ctx);
        
        var paramsFromUri = ctx.Request.QueryString;


        if(endpoint is null)
        {
            await Return404(ctx);
            return;
        }
        var command = endpoint.Command;
        var commandType = command.GetType();

        var fixedParams = paramsFromUri.AllKeys.Select(c => c.ToLower()).ToList();
        
        foreach (var field in commandType.GetProperties())
        {
            var value = paramsFromUri[fixedParams.FirstOrDefault(c => c == field.Name.ToLower())];
            command = ParamsParser.ParseToType(field, value, field.PropertyType, command);
        }
        var responseValue = endpoint.Handler!.GetType().GetMethods()[0].Invoke(endpoint.Handler, new object?[]
        {
            command
        });
        var methodToResolve = MethodToResolve.MakeGenericMethod(endpoint.OutputType);
        var resolvedTask = methodToResolve.Invoke(this, new object?[]
        {
            responseValue
        });
        
        
        var response = JsonConvert.SerializeObject(resolvedTask);
        using var resp = ctx.Response;
        resp.StatusCode = (int) HttpStatusCode.Accepted;
        resp.StatusDescription = "OK";
        var byteResponse = Encoding.UTF8.GetBytes(response);
        resp.ContentLength64 = byteResponse.Length;
        resp.OutputStream.Write(byteResponse);
        var output = resp.OutputStream;
        await output.WriteAsync(byteResponse,0,byteResponse.Length);
        output.Close();
    }
    private static async Task Return404(HttpListenerContext c)
    {
        using HttpListenerResponse resp = c.Response;
        resp.StatusCode = (int) HttpStatusCode.NotFound;
        resp.StatusDescription = "Not Found";
        await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Not Found"));
    }
    public static object ResolveTask<T>(Task<T?> obj) => obj.Result!;
}