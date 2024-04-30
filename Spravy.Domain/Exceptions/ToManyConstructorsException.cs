namespace Spravy.Domain.Exceptions;

public class ToManyConstructorsException : Exception
{
    public ToManyConstructorsException(Type type, int expectedCount, int count) : base(
        $"Type {type} have {count} constructors, expected count {expectedCount}.")
    {
        Type = type;
        Count = count;
        ExpectedCount = expectedCount;
    }

    public Type Type { get; }
    public int Count { get; }
    public int ExpectedCount { get; }
}