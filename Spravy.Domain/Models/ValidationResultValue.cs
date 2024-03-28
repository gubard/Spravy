namespace Spravy.Domain.Models;

public abstract class ValidationResultValue<TValue> : ValidationResult
{
    protected ValidationResultValue(Guid id, string name, TValue value) : base(id, name)
    {
        Value = value;
    }

    public TValue Value { get; }
}