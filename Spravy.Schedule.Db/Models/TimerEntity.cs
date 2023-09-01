namespace Spravy.Schedule.Db.Models;

public class TimerEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset DueDateTime { get; set; }
    public Guid EventId { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();
}