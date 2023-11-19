using System.Linq;

namespace myapi.core;

using System;
using System.Collections.Generic;
using System.Reflection;

public static class ClassHierarchyExplorer
{
    public static List<Type> GetClassesImplementingInterface(Type interfaceType, Assembly assembly)
    {
        List<Type> implementingClasses = new List<Type>();
        
        foreach (Type type in assembly.GetTypes())
        {
            if (type.GetInterfaces().Any(c => c == interfaceType) )
            {
                implementingClasses.Add(type);
            }
        }
        
        return implementingClasses;
    }
}