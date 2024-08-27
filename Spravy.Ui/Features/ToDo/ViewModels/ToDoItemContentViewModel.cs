namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemContentViewModel : ViewModelBase
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string link = string.Empty;

    [ObservableProperty]
    private ToDoItemType type;

    public AvaloniaList<string> Names { get; } = new();
}
