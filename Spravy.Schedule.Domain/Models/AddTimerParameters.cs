namespace Spravy.Schedule.Domain.Models;

public readonly struct AddTimerParameters
{
    public AddTimerParameters(DateTime dueDateTime, Guid eventId, ReadOnlyMemory<byte> content)
    {
        DueDateTime = dueDateTime;
        EventId = eventId;
        Content = content;
    }

    public DateTime DueDateTime { get; }
    public Guid EventId { get; }
    public ReadOnlyMemory<byte> Content { get; }
}