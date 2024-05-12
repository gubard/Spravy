using Spravy.Domain.Errors;
using Spravy.Domain.Models;

namespace Spravy.Db.Errors;

public class NotFoundEntityError : Error
{
    public static readonly Guid MainId = new("FC0579CC-DD09-4C3C-B631-7E97B590BA22");

    protected NotFoundEntityError() : base(MainId)
    {
        Type = string.Empty;
        Key = string.Empty;
    }

    public NotFoundEntityError(string type, string key) : base(MainId)
    {
        Type = type;
        Key = key;
    }

    public string Type { get; protected set; }
    public string Key { get; protected set; }

    public override string Message
    {
        get => $"Not found entity {Type} by id {Key}";
    }
}