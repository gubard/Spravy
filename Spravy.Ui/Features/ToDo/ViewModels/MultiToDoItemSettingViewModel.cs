using Spravy.Core.Mappers;

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
    private string name = string.Empty;

    [ObservableProperty]
    private string link = string.Empty;

    [ObservableProperty]
    private ToDoItemType type;

    [ObservableProperty]
    private DateOnly dueDate = DateTime.Now.ToDateOnly();

    [ObservableProperty]
    private bool isDueDate;

    public MultiToDoItemSettingViewModel(ToDoItemEntityNotify item)
    {
        Item = item;
        ToDoItemTypes = new(UiHelper.ToDoItemTypes.ToArray());
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }
    public ToDoItemEntityNotify Item { get; }
}
