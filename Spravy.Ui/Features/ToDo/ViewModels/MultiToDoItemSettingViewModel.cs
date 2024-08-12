namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class MultiToDoItemSettingViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isName;

    [ObservableProperty]
    private bool isLink;

    [ObservableProperty]
    private bool isType;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string link;

    [ObservableProperty]
    private ToDoItemType type;

    public MultiToDoItemSettingViewModel(ToDoItemEntityNotify item)
    {
        Item = item;
        Name = string.Empty;
        Link = string.Empty;
        ToDoItemTypes = new(UiHelper.ToDoItemTypes.ToArray());
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }
    public ToDoItemEntityNotify Item { get; }
}
