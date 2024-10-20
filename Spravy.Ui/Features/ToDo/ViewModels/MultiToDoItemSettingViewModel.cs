namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class MultiToDoItemSettingViewModel : DialogableViewModelBase, IApplySettings
{
    private readonly ReadOnlyMemory<ToDoItemEntityNotify> items;
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private bool isName;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool isLink;

    [ObservableProperty]
    private string link = string.Empty;

    [ObservableProperty]
    private bool isType;

    [ObservableProperty]
    private ToDoItemType type;

    [ObservableProperty]
    private bool isDueDate;

    [ObservableProperty]
    private DateOnly dueDate = DateTime.Now.ToDateOnly();

    [ObservableProperty]
    private bool isRemindDaysBefore;

    [ObservableProperty]
    private uint remindDaysBefore;

    public MultiToDoItemSettingViewModel(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        IToDoService toDoService
    )
    {
        this.items = items;
        this.toDoService = toDoService;
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

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.EditToDoItemsAsync(GetEditToDoItems(), ct);
    }

    private EditToDoItems GetEditToDoItems()
    {
        var result = new EditToDoItems().SetIds(items.Select(x => x.Id));

        if (IsName)
        {
            result = result.SetName(new(Name));
        }

        if (IsType)
        {
            result = result.SetType(new(Type));
        }

        if (IsLink)
        {
            result = result.SetLink(new(Link.ToOptionUri()));
        }

        if (IsDueDate)
        {
            result = result.SetDueDate(new(DueDate));
        }

        if (IsRemindDaysBefore)
        {
            result = result.SetRemindDaysBefore(new(RemindDaysBefore));
        }

        return result;
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}
