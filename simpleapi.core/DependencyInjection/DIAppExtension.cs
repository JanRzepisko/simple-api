namespace simpleapi.core.DependencyInjection;

public static class DIAppExtension
{
    
    public static App.App RegisterSingletonService<TService>(this App.App app, TService instance) where TService : class
    {
        app.SingletonServices.Add(new SingletonService(instance));
        return app;
    }
    public static App.App RegisterMultiService<ITService, TService>(this App.App app) where ITService : class where TService : class, ITService
    {
        app.MultiServices.Add(new MultiService(typeof(ITService), typeof(TService)));
        return app;
    }
}