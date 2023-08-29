namespace Spravy.Domain.Models;

public readonly struct IsSuccessValue<TValue>
{
    public IsSuccessValue(TValue value)
    {
        IsSuccess = true;
        Value = value;
    }

    public bool IsSuccess { get; }
    public TValue Value { get; }
}