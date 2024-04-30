using Spravy.Domain.Errors;
using Spravy.Domain.Models;

namespace Spravy.Db.Errors;

public class NotFoundEntityError : Error
{
    public static readonly Guid MainId = new("FC0579CC-DD09-4C3C-B631-7E97B590BA22");

    protected NotFoundEntityError() : base(MainId)
    {
        Type = typeof(UnknownType);
        Key = new();
    }

    public NotFoundEntityError(Type type, object key) : base(MainId)
    {
        Type = type;
        Key = key;
    }

    public Type Type { get; protected set; }
    public object Key { get; protected set; }

    public override string Message
    {
        get => $"Not found entity {Type} by id {Key}";
    }
}