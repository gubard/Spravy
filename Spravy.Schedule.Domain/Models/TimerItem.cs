namespace Spravy.Schedule.Domain.Models;

public readonly struct TimerItem
{
    public TimerItem(DateTime dueDateTime, Guid eventId, ReadOnlyMemory<byte> content, Guid id)
    {
        DueDateTime = dueDateTime;
        EventId = eventId;
        Content = content;
        Id = id;
    }

    public Guid Id { get; }
    public DateTime DueDateTime { get; }
    public Guid EventId { get; }
    public ReadOnlyMemory<byte> Content { get; }
}
