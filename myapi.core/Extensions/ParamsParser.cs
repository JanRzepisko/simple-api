using System.Globalization;
using System.Reflection;
using myapi.core.Exceptions;

namespace myapi.core.Extensions;

public static class ParamsParser
{
    public static object ParseToType(PropertyInfo field, string? value, Type commandPropertyType,object command)
    {
       if (value is null)
        {
            field.SetValue(command, null);
            return command;
        }
        dynamic fixedValue;
        
        if (commandPropertyType == typeof(int))
        {
           if(!int.TryParse(value.ToString(), CultureInfo.InvariantCulture, out var result))
               throw new InvalidTypeException();
           fixedValue = result;
        }
        else if (commandPropertyType == typeof(double))
        {
           if(!double.TryParse(value.ToString(), CultureInfo.InvariantCulture, out var result))
              throw new InvalidTypeException();
           fixedValue = result;
        }
        else if (commandPropertyType == typeof(decimal))
        {
           if(!decimal.TryParse(value.ToString(), CultureInfo.InvariantCulture, out var result))
              throw new InvalidTypeException();
           fixedValue = result;
        }
        else if (commandPropertyType == typeof(float))
        {
           if(!float.TryParse(value.ToString(), CultureInfo.InvariantCulture, out var result))
              throw new InvalidTypeException();
           fixedValue = result;
        }
        else if (commandPropertyType == typeof(bool))
        {
           if(!bool.TryParse(value.ToString(), out var result))
              throw new InvalidTypeException();
           fixedValue = result;
        }
        else if (commandPropertyType == typeof(char))
        {
           if(!char.TryParse(value.ToString(), out var result))
              throw new InvalidTypeException();
           fixedValue = result;
        }
        else if (commandPropertyType == typeof(char))
        {
           if(!char.TryParse(value.ToString(), out var result))
              throw new InvalidTypeException();
           fixedValue = result;
        }
        else if (commandPropertyType == typeof(byte))
        {
           if(!byte.TryParse(value.ToString(), CultureInfo.InvariantCulture, out var result))
              throw new InvalidTypeException();
           fixedValue = result;
        }
        else
        {
           fixedValue = value!;
        }

        field.SetValue(command,  fixedValue!);

        return command;
    }
}