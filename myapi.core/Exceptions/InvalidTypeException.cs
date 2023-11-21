using System;

namespace myapi.core.Exceptions;

public class InvalidTypeException : Exception
{
    public InvalidTypeException(string message) : base("Invalid type " + message) {}
    public InvalidTypeException() : base("Invalid type") {}

}