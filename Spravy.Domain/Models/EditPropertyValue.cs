namespace Spravy.Domain.Models;

public readonly struct EditPropertyValue<TValue>
{
    public EditPropertyValue(TValue value)
    {
        Value = value;
        IsEdit = true;
    }

    public bool IsEdit { get; }
    public TValue Value { get; }
}