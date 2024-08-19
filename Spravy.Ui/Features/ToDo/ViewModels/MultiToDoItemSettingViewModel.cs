namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class MultiToDoItemSettingViewModel(ToDoItemEntityNotify item) : ViewModelBase
{
    [ObservableProperty]
    private bool isName;

    [ObservableProperty]
    private bool isLink;

    [ObservableProperty]
    private bool isType;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string link = string.Empty;

    [ObservableProperty]
    private ToDoItemType type;

    [ObservableProperty]
    private DateOnly dueDate = DateTime.Now.ToDateOnly();

    [ObservableProperty]
    private bool isDueDate;

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; } =
        new(UiHelper.ToDoItemTypes.ToArray());
    public ToDoItemEntityNotify Item { get; } = item;
}
