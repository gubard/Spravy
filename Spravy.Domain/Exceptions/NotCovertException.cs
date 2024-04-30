namespace Spravy.Domain.Exceptions;

public class NotCovertException : Exception
{
    public NotCovertException(Type currentType, Type expectedType) : base(
        $"Can't convert {currentType} to {expectedType}.")
    {
        CurrentType = currentType;
        ExpectedType = expectedType;
    }

    public Type CurrentType { get; }
    public Type ExpectedType { get; }
}