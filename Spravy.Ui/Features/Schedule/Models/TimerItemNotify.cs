namespace Spravy.Ui.Features.Schedule.Models;

public class TimerItemNotify : NotifyBase
{
    public TimerItemNotify(Guid id, DateTime dueDateTime, Guid eventId, string name)
    {
        Id = id;
        DueDateTime = dueDateTime;
        Name = name;
    }

    public Guid Id { get; }
    public DateTime DueDateTime { get; }
    public string Name { get; }
}
