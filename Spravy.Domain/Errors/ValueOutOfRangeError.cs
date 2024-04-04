namespace Spravy.Domain.Errors;

public class ValueOutOfRangeError : Error
{
    public static readonly Guid MainId = new("95E4337C-DFA4-415F-8AD3-D528B8294610");

    public ValueOutOfRangeError(object value) : base(MainId, $"Value {value} out of range")
    {
        Value = value;
    }

    public object Value { get; protected set; }
}