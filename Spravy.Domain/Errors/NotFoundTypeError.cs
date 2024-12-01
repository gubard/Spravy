namespace Spravy.Domain.Errors;

public class NotFoundTypeError : Error
{
    public static readonly Guid MainId = new("495293DE-213C-4F8C-93A6-FBE5732BA815");

    protected NotFoundTypeError() : base(MainId)
    {
        Type = TypeCache<UnknownType>.Type;
    }

    public NotFoundTypeError(Type type) : base(MainId)
    {
        Type = type;
    }

    public Type Type { get; protected set; }

    public override string Message => $"Not found {Type}";
}