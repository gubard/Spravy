namespace Spravy.Domain.Models;

public abstract class ValidationResultValue<TValue> : ValidationResult
{
    protected ValidationResultValue(Guid id, TValue value) : base(id)
    {
        Value = value;
    }

    public TValue Value { get; }
}