namespace Spravy.Domain.Exceptions;

public class NotFondConstructorException : Exception
{
    public NotFondConstructorException(Type type, Type typeParameter)
        : base($"Type {type} not have constructor with parameter type {typeParameter}.")
    {
        Type = type;
        TypeParameter = typeParameter;
    }

    public Type Type { get; }
    public Type TypeParameter { get; }
}
