# simple-api project

NuGet: [NuGet Gallery](https://www.nuget.org/packages/simpleapi.core)

**simple-api** Project is an attempt to create a framework for creating very transparent in terms of code and efficient API-type applications. In accordance with the principle of Domain Driven Design, each endpoint is a separate class, which allows for very simple work on the entire project. 

The framework allows you to 

 - Creating API interface (http/https)
 - Middlewares implementations
 - Dependency injection *
 - Simple UI for testing and creating documentation *.
 - Web page construction *


# Implementation

### Construction of the API interface 
If you want to start a server 

    IApp app = App.Init<Program>(5050)
    app.Run();

### Construction of Endpoints

    [Api("/example", Method.GET)]  
	public static class ExampleEndpoint  
	{  
	    public class Command  
		{  
	        public int A { get; set; }  
	        public int B { get; set; }  
	        public int C { get; set; }  
	    }  

	    public class Handler : IEndpoint<Command, int>  
		{  
	        public async Task<int> Handle(Command command)  
	        {  
		        //TODO all features
	            return command.A + command.B + command.C;  
	        }  
	    }  
	}

Command is a class that has a request body in it if it is a GET it will be taken from the parameters in the URL, if it is a method that has a body it will be built from JSON

###  Construction of Middleware
if you want to create middleware you must instantiate the class 
```csharp
    public class ExampleMiddleware : IMiddleware  
	{  
	    public async Task<HttpListenerContext> Invoke(HttpListenerContext ctx)  
	    {  
	        Console.WriteLine("TODO somethink cool");  
	        return ctx;  
	    }  
	}
```

And then you have two options

-  Register middleware witch runs before endpoint actions
	```csharp
	IApp app = App.Init<Program>(5050)  
 		.RegisterPreMiddleware<ExampleMiddlewareBeforeEndpoint>()
	```
 -   Register middleware witch runs after endpoint actions
 	```csharp
	IApp app = App.Init<Program>(5050)  
    		.RegisterPostMiddleware<ExampleMiddlewareAfterEndpoint>()
	```

## Services

The sites are divided into Singletons and Multi sites.
Singletons hundred objects having only one instance, built at the start of the project. Type of service can occur only once during registration

### Register Singleton Service

if you want to add a singleton service to your application you must add to the app object

 1. Option 1: Use interface and type of object

	```csharp
	IApp app = App.Init<Program>(5050)
		.RegisterSingletonService<IExampleService, ExampleService>();
	``` 
			   
 2. Option 2: Use Type of interface and ready instance

	   ```csharp
	   var serivce = new ExampleService();
	   IApp app = App.Init<Program>(5050)
		   .RegisterSingletonService<IExampleService>(service);
	   ```

 3. Use only instance without interface
	```csharp
	var serivce =  new ExampleService();
	IApp app = App.Init<Program>(5050)
		.RegisterSingletonService<IExampleService>(service);
	```

### Multi Services

The implementation of multiservices has 2 variants

 1. Use instance type and interface to create per all requests
	```csharp
	IApp app = App.Init<Program>(5050)  
	    .RegisterMultiService<IExampleService, ExampleService>();
	```
 2. Or use only instance type
	```csharp
	IApp app = App.Init<Program>(5050)  
	    .RegisterMultiService<ExampleService>();
	```
### Use Services

In order to use the registered services in the Handler class of a given endpoint, you must create a constructor containing the registered services

```csharp
[Api("/example", Method.GET)]  
public static class ExampleEndpoint  
{  
    public class Command  
	{

	}  
    public class Handler : IEndpoint<Command, int>  
    {  
        readonly IExampleService _exampleService;  
        public Handler(IExampleService exampleService)  
        {  
            _exampleService = exampleService;  
        }  
          
        public async Task<int> Handle(Command command)  
        {  
            return _exampleService.TwoPlusTwo();  
        }  
    }  
}
```
 
