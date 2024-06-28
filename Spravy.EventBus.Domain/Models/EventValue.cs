namespace Spravy.EventBus.Domain.Models;

public readonly struct EventValue
{
    public EventValue(Guid id, ReadOnlyMemory<byte> content)
    {
        Id = id;
        Content = content;
    }

    public Guid Id { get; }
    public ReadOnlyMemory<byte> Content { get; }
}
