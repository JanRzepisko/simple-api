namespace simpleapi.example.Services;

public class ExampleService : IExampleService
{
    
    int sum = 0;
    public int TwoPlusTwo()
    {
        sum += 2;
        return sum;
    }
}