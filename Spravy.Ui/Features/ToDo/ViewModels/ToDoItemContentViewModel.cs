namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemContentViewModel : IconViewModel
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string link = string.Empty;

    [ObservableProperty]
    private ToDoItemType type;

    [ObservableProperty]
    private Color color;

    public ToDoItemContentViewModel(IObjectStorage objectStorage)
        : base(objectStorage) { }

    public AvaloniaList<string> Names { get; } = new();
    public override string ViewId => TypeCache<ToDoItemContentViewModel>.Type.Name;
}
