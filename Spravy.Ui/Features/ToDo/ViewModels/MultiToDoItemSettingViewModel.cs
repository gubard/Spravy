namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class MultiToDoItemSettingViewModel : DialogableViewModelBase
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

    public MultiToDoItemSettingViewModel()
    {
        ToDoItemTypes = new(UiHelper.ToDoItemTypes.ToArray());
    }

    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }

    public override string ViewId
    {
        get => $"{TypeCache<ToDoItemToStringSettingsViewModel>.Type}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
