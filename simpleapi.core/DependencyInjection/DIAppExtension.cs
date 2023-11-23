using simpleapi.core.Exceptions;

namespace simpleapi.core.DependencyInjection;

public static class DiAppExtension
{
    public static App.App RegisterSingletonService<TService>(this App.App app, TService instance) where TService : class
    {
        app.ValidateService(typeof(TService));
        app.SingletonServices.Add(new SingletonService(instance));
        return app;
    }
    public static App.App RegisterSingletonService<TIService, TService>(this App.App app, TService instance) where TService : class where TIService : class
    {
        app.ValidateService(typeof(TService), typeof(TIService));
        app.SingletonServices.Add(new SingletonService(instance, typeof(TIService)));
        return app;
    }
    public static App.App RegisterSingletonService<TIService, TService>(this App.App app) where TService : class where TIService : class
    {
        app.ValidateService(typeof(TService), typeof(TIService));
        var instance = Activator.CreateInstance<TService>();
        app.SingletonServices.Add(new SingletonService(instance, typeof(TIService)));
        return app;
    }
    public static App.App RegisterMultiService<TIService, TService>(this App.App app) where TIService : class where TService : class, TIService
    {
        app.ValidateService(typeof(TService), typeof(TIService));
        if (app.SingletonServices.Any(c => c == new SingletonService(typeof(TIService))))
            throw new ServiceCantBeRegistered($"Service of type {typeof(TIService).Name} exist");
        app.MultiServices.Add(new MultiService(typeof(TIService), typeof(TService)));
        return app;
    }
    
    public static App.App RegisterMultiService<TService>(this App.App app) where TService : class
    {
        app.ValidateService(typeof(TService));
        app.MultiServices.Add(new MultiService(null, typeof(TService)));
        return app;
    }

    private static void ValidateService(this App.App app, Type service, Type? @interface = null)
    {
        if (app.SingletonServices.Any(c => c.Instance.GetType() == service))
            throw new ServiceCantBeRegistered($"Service of type {service.Name} exist");
        if (app.SingletonServices.Any(c => c.Interface == @interface) && @interface is not null)
            throw new ServiceCantBeRegistered($"Service of type {service.Name} exist");
        if (app.MultiServices.Any(c => c.Interface == @interface) && @interface is not null)
            throw new ServiceCantBeRegistered($"Service of type {service.Name} exist");
        if (app.MultiServices.Any(c => c.Implementation == service))
            throw new ServiceCantBeRegistered($"Service of type {service.Name} exist");
    }
}