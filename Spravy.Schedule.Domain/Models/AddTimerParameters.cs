namespace Spravy.Schedule.Domain.Models;

public readonly struct AddTimerParameters
{
    public AddTimerParameters(DateTime dueDateTime, Guid eventId, ReadOnlyMemory<byte> content, string name)
    {
        DueDateTime = dueDateTime;
        EventId = eventId;
        Content = content;
        Name = name;
    }

    public DateTime DueDateTime { get; }
    public Guid EventId { get; }
    public ReadOnlyMemory<byte> Content { get; }
    public string Name { get; }
}