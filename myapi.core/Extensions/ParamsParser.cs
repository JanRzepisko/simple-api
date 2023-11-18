using System.Globalization;
using System.Reflection;

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
           fixedValue = int.Parse(value.ToString(), CultureInfo.InvariantCulture) ;
        }
        else if (commandPropertyType == typeof(double))
        {
           fixedValue = double.Parse(value.ToString(), CultureInfo.InvariantCulture) ;
        }
        else if (commandPropertyType == typeof(decimal))
        {
           fixedValue = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        }
        else if (commandPropertyType == typeof(float))
        {
           fixedValue = float.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        else if (commandPropertyType == typeof(bool))
        {
           fixedValue = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        }
        else if (commandPropertyType == typeof(char))
        {
           fixedValue = Convert.ToChar(value, CultureInfo.InvariantCulture);
        }
        else if (commandPropertyType == typeof(uint))
        {
           fixedValue = Convert.ToUInt32(value, CultureInfo.InvariantCulture);
        }
        else if (commandPropertyType == typeof(short))
        {
           fixedValue = short.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        else if (commandPropertyType == typeof(byte))
        {
           fixedValue = Convert.ToByte(value, CultureInfo.InvariantCulture);
        }
        else
        {
           fixedValue = value!;
        }

        field.SetValue(command,  fixedValue!);

        return command;
    }
}