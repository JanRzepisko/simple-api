using simpleapi.core.Enums;

namespace simpleapi.core.Attributes;


[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)
]
public class ApiAttribute : System.Attribute
{
    private string Path;
    private Method Method;
    public ApiAttribute(string path,Method method)
    {
        Path = path;
        Method = method;
    }
}