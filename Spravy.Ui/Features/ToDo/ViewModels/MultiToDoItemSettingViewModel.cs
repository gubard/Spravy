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

    public MultiToDoItemSettingViewModel()
    {
        Name = string.Empty;
        Link = string.Empty;
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }
    public Guid ToDoItemId { get; set; }
}
