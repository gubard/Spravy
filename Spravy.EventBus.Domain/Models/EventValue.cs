namespace Spravy.EventBus.Domain.Models;

public readonly struct EventValue
{
    public EventValue(Guid id, Stream stream)
    {
        Id = id;
        Stream = stream;
    }

    public Guid Id { get; }
    public Stream Stream { get; }
}