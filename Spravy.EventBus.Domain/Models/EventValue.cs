namespace Spravy.EventBus.Domain.Models;

public readonly struct EventValue
{
    public EventValue(Guid id, byte[] content)
    {
        Id = id;
        Content = content;
    }

    public Guid Id { get; }
    public byte[] Content { get; }
}