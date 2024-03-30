namespace Spravy.Domain.ValidationResults;

public abstract class ErrorValue<TValue> : Error
{
    protected ErrorValue(Guid id, string name, TValue value) : base(id, name)
    {
        Value = value;
    }

    public TValue Value { get; }
}