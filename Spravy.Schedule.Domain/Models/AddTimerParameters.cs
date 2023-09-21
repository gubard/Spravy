namespace Spravy.Schedule.Domain.Models;

public readonly struct AddTimerParameters
{
    public AddTimerParameters(DateTimeOffset dueDateTime, Guid eventId, byte[] content)
    {
        DueDateTime = dueDateTime;
        EventId = eventId;
        Content = content;
    }

    public DateTimeOffset DueDateTime { get; }
    public Guid EventId { get; }
    public byte[] Content { get; }
}