namespace Spravy.Schedule.Domain.Models;

public readonly struct TimerItem
{
    public TimerItem(DateTimeOffset dueDateTime, Guid eventId, Stream content, Guid id)
    {
        DueDateTime = dueDateTime;
        EventId = eventId;
        Content = content;
        Id = id;
    }

    public Guid Id { get; }
    public DateTimeOffset DueDateTime { get; }
    public Guid EventId { get; }
    public Stream Content { get; }
}