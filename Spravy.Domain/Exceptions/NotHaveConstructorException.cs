namespace Spravy.Domain.Exceptions;

public class NotHaveConstructorException : Exception
{
    public NotHaveConstructorException(Type type) : base($"Type {type} not have constructor.")
    {
        Type = type;
    }

    public Type Type { get; }
}