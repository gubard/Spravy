namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemToStringSettingsViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IClipboardService clipboardService;
    private readonly AvaloniaList<CheckedItem<ToDoItemStatus>> statuses = new();
    private readonly IToDoService toDoService;

    public ToDoItemToStringSettingsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        IToDoService toDoService,
        IClipboardService clipboardService
    ) : base(item, items)
    {
        this.toDoService = toDoService;
        this.clipboardService = clipboardService;

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

        return ResultItems.ToResult()
           .IfSuccessForEach(x => new ToDoItemToStringOptions(status, x.CurrentId).ToResult())
           .IfSuccessAsync(
                options => toDoService.ToDoItemToStringAsync(options, ct)
                   .IfSuccessAsync(text => clipboardService.SetTextAsync(text, ct), ct),
                ct
            );
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