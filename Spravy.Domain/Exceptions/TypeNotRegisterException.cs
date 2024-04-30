namespace Spravy.Domain.Exceptions;

public class TypeNotRegisterException : Exception
{
    public TypeNotRegisterException(Type type) : base($"Type {type} not register.")
    {
        Type = type;
    }

    public Type Type { get; }
}