namespace Spravy.EventBus.Db.Models;

public class EventEntity
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
}