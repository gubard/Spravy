namespace Spravy.Schedule.Db.Models;

public class TimerEntity
{
    public Guid Id { get; set; }
    public DateTime DueDateTime { get; set; }
    public Guid EventId { get; set; }
    public byte[] Content { get; set; } = [];
    public string Name { get; set; } = string.Empty;
}
