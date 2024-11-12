namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddToDoItemViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IObjectStorage objectStorage;
    private readonly IToDoService toDoService;

    public AddToDoItemViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IObjectStorage objectStorage,
        IToDoService toDoService,
        EditToDoItemViewModel editToDoItemViewModel
    )
        : base(editItem, editItems)
    {
        this.objectStorage = objectStorage;
        this.toDoService = toDoService;
        EditToDoItemViewModel = editToDoItemViewModel;
    }

    public EditToDoItemViewModel EditToDoItemViewModel { get; }

    public override string ViewId
    {
        get =>
            EditItem.TryGetValue(out var editItem)
                ? $"{TypeCache<AddToDoItemViewModel>.Type.Name}:{editItem.Id}"
                : $"{TypeCache<AddToDoItemViewModel>.Type.Name}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<AddToDoItemViewModelSettings>(ViewId, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            EditToDoItemViewModel.SetItem(setting.EditToDoItemViewModelSettings);

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            )
            .IfSuccessAsync(() => EditToDoItemViewModel.LoadStateAsync(ct), ct);
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage
            .SaveObjectAsync(ViewId, new AddToDoItemViewModelSettings(this), ct)
            .IfSuccessAsync(() => EditToDoItemViewModel.SaveStateAsync(ct), ct);
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return Result
            .AwaitableSuccess.IfSuccessAsync(
                () =>
                {
                    if (ResultIds.IsEmpty)
                    {
                        return toDoService.AddToDoItemAsync(
                            new[]
                            {
                                EditToDoItemViewModel.GetAddToDoItemOptions(
                                    OptionStruct<Guid>.Default
                                ),
                            },
                            ct
                        );
                    }

                    return ResultIds
                        .ToResult()
                        .IfSuccessForEach(x =>
                            EditToDoItemViewModel.GetAddToDoItemOptions(x.ToOption()).ToResult()
                        )
                        .IfSuccessAsync(options => toDoService.AddToDoItemAsync(options, ct), ct);
                },
                ct
            )
            .ToResultOnlyAsync();
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}
