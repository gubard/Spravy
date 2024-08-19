using Spravy.Domain.Errors;

namespace Spravy.EventBus.Domain.Errors;

public class NotFoundEventError : Error
{
    public static readonly Guid MainId = new("4F5D4EA1-5012-4AE2-991E-A47C979BBF42");

    protected NotFoundEventError()
        : base(MainId) { }

    public NotFoundEventError(Guid eventId)
        : base(MainId)
    {
        EventId = eventId;
    }

    public Guid EventId { get; protected set; }

    public override string Message
    {
        get => $"Not found event {EventId}";
    }
}
