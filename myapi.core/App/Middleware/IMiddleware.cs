using System.Threading.Tasks;

namespace myapi.core.Middlewares;

public interface IMiddleware
{
    Task Invoke();
}