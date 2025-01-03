namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemToStringSettingsViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IClipboardService clipboardService;
    private readonly AvaloniaList<CheckedItem<ToDoItemStatus>> statuses = new();
    private readonly IToDoUiService toDoUiService;

    public ToDoItemToStringSettingsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        IClipboardService clipboardService,
        IToDoUiService toDoUiService
    ) : base(item, items)
    {
        this.clipboardService = clipboardService;
        this.toDoUiService = toDoUiService;

        var select = UiHelper.ToDoItemStatuses
           .ToArray()
           .Select(
                x => new CheckedItem<ToDoItemStatus>
                {
                    Item = x,
                    IsChecked = true,
                }
            );

        statuses.AddRange(select);
    }

    public IEnumerable<CheckedItem<ToDoItemStatus>> Statuses => statuses;

    public override string ViewId => TypeCache<ToDoItemToStringSettingsViewModel>.Name;

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        var status = Statuses.Where(x => x.IsChecked).Select(x => x.Item).ToArray();

        return toDoUiService.GetRequest(
                GetToDo.WithDefaultItems.SetToStringItems(
                    new GetToStringItem[]
                    {
                        new(ResultIds, status),
                    }
                ),
                ct
            )
           .IfSuccessAsync(
                response => response.ToStringItems.Select(x => x.Text).JoinString(Environment.NewLine).ToResult(),
                ct
            )
           .IfSuccessAsync(text => clipboardService.SetTextAsync(text, ct), ct);
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}