namespace Spravy.Domain.Exceptions;

public class NotEqualsException<TValue> : Exception
{
    public NotEqualsException(string name, TValue value, TValue expected)
        : base($"{name} equals {value} expected {expected}.")
    {
        Name = name;
        Value = value;
        Expected = expected;
    }

    public string Name { get; }
    public TValue Value { get; }
    public TValue Expected { get; }
}
