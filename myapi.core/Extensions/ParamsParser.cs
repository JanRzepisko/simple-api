using System.Reflection;

namespace myapi.core.Extensions;

public static class ParamsParser
{
    public static object ParseToType(PropertyInfo field, string? value, Type commandPropertyType,object command)
    {
        if (value is null)
            field.SetValue(command, null);

            
        if (commandPropertyType == typeof(int))
            field.SetValue(command, int.Parse(value));
        else if (commandPropertyType == typeof(double))
            field.SetValue(command,  double.Parse(value));
        else if (commandPropertyType == typeof(decimal))
            field.SetValue(command,  decimal.Parse(value));
        else if (commandPropertyType == typeof(float))
            field.SetValue(command,  float.Parse(value));
        else if (commandPropertyType == typeof(bool))
            field.SetValue(command,  bool.Parse(value));        
        else if (commandPropertyType == typeof(char))
            field.SetValue(command,  bool.Parse(value));        
        else if (commandPropertyType == typeof(uint))
            field.SetValue(command,  bool.Parse(value));
        else if (commandPropertyType == typeof(short))
            field.SetValue(command,  bool.Parse(value));
        else if (commandPropertyType == typeof(byte))
            field.SetValue(command,  byte.Parse(value));
        else
            field.SetValue(command,  value!);

        return command;
    }
}