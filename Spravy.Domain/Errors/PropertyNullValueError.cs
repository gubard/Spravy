namespace Spravy.Domain.Errors;

public class PropertyNullValueError : Error
{
    public static readonly Guid MainId = new("D22DFA98-E914-49B6-9A35-12E8EC1D314D");

    protected PropertyNullValueError() : base(MainId, "PropertyNullValue")
    {
    }

    public PropertyNullValueError(string propertyName) : base(MainId, "PropertyNullValue")
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; protected set; }
}