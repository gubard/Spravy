namespace Spravy.Domain.Errors;

public abstract class ErrorValue<TValue> : Error
{
    protected ErrorValue(Guid id, TValue value) : base(id)
    {
        Value = value;
    }

    public TValue Value { get; }
}