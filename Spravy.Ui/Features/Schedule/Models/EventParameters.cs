namespace Spravy.Ui.Features.Schedule.Models;

public class EventParameters
{
    public EventParameters(Guid id, EventType type)
    {
        Id = id;
        Type = type;
    }

    public EventType Type { get; }
    public Guid Id { get; }
}
