namespace Spravy.Domain.Errors;

public class PropertyEmptyStringError : Error
{
    public static readonly Guid MainId = new("2688D7D6-83A6-4499-8613-48F4D4A8EF0B");

    protected PropertyEmptyStringError() : base(MainId)
    {
        PropertyName = string.Empty;
    }

    public PropertyEmptyStringError(string propertyName) : base(MainId)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; protected set; }

    public override string Message
    {
        get => $"Property {PropertyName} can't be empty";
    }
}