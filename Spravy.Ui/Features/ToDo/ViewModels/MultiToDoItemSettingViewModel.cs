namespace Spravy.Ui.Features.ToDo.ViewModels;

public class MultiToDoItemSettingViewModel : ViewModelBase
{
    public MultiToDoItemSettingViewModel()
    {
        Name = string.Empty;
        Link = string.Empty;
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }
    public Guid ToDoItemId { get; set; }

    [Reactive]
    public bool IsName { get; set; }

    [Reactive]
    public bool IsLink { get; set; }

    [Reactive]
    public bool IsType { get; set; }

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public string Link { get; set; }

    [Reactive]
    public ToDoItemType Type { get; set; }
}
