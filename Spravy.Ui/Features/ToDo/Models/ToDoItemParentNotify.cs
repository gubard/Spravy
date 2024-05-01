namespace Spravy.Ui.Features.ToDo.Models;

public class ToDoItemParentNotify : NotifyBase
{
    private Guid id;
    private string name = string.Empty;

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }
}