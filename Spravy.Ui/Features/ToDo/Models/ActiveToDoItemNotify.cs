namespace Spravy.Ui.Features.ToDo.Models;

public class ActiveToDoItemNotify : NotifyBase
{
    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public string Name { get; set; }
}