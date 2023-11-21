# simple-api project

NuGet: [https://www.nuget.org/packages/simpleapi.core](Nuget Gallery)

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

    public class ExampleMiddleware : IMiddleware  
	{  
	    public async Task<HttpListenerContext> Invoke(HttpListenerContext ctx)  
	    {  
	        Console.WriteLine("TODO somethink cool");  
	        return ctx;  
	    }  
	}	

And then register it on the App object

    IApp app = App.Init<Program>(5050)  
	    .AddMiddleware<ExampleMiddleware>();
