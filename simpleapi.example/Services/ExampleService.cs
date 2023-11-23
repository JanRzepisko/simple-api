namespace simpleapi.example.Services;

public class ExampleService : IExampleService
{
    
    int sum = 0;
    public int AddTwo()
    {
        sum += 2;
        return sum;
    }
}