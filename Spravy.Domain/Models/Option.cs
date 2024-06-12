namespace Spravy.Domain.Models;

public readonly struct Option<TValue> where TValue : class
{
    public Option(TValue? value)
    {
        Value = value;
        IsHasValue = Value is not null;
    }
    
    public bool IsHasValue { get; }
    public TValue? Value { get; }
    
    public bool TryGetValue([MaybeNullWhen(false)] out TValue value)
    {
        if (IsHasValue)
        {
            value = Value!;
            
            return true;
        }
        
        value = null;
        
        return false;
    }
}

public readonly struct OptionStruct<TValue> where TValue : struct
{
    public OptionStruct(TValue? value)
    {
        Value = value;
        IsHasValue = Value is not null;
    }
    
    public bool IsHasValue { get; }
    public TValue? Value { get; }
    
    public bool TryGetValue(out TValue value)
    {
        if (IsHasValue)
        {
            value = Value!.Value;
            
            return true;
        }
        
        value = default;
        
        return false;
    }
}