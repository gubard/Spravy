namespace Spravy.Domain.Errors;

public abstract class ValueOutOfRangeError<TValue> : Error
{
    public ValueOutOfRangeError(TValue value, Guid id) : base(id)
    {
        Value = value;
    }

    public TValue Value { get; protected set; }

    public override string Message => $"Value {Value} is out of range.";
}