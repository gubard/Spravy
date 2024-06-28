namespace Spravy.Domain.Errors;

public class PropertyWhiteSpaceStringError : Error
{
    public static readonly Guid MainId = new("890C8ABC-6D54-49BC-A947-3B08D698AD31");

    protected PropertyWhiteSpaceStringError()
        : base(MainId)
    {
        PropertyName = string.Empty;
    }

    public PropertyWhiteSpaceStringError(string propertyName)
        : base(MainId)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; protected set; }

    public override string Message
    {
        get => $"Property {PropertyName} can't be white space";
    }
}
