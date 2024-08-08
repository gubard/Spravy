namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ReferenceToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private ToDoItemEntityNotify? item;

    public ReferenceToDoItemSettingsViewModel(
        ToDoItemSelectorViewModel toDoItemSelector,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        ToDoItemSelector = toDoItemSelector;
        this.toDoService = toDoService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }
    public ToDoItemSelectorViewModel ToDoItemSelector { get; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return Item.IfNotNull(nameof(Item))
            .IfSuccessAsync(i => toDoService.GetToDoItemAsync(i.Id, ct), ct)
            .IfSuccessAsync(
                i =>
                {
                    ToDoItemSelector.IgnoreIds = new([i.Id,]);

                    if (i.ReferenceId.TryGetValue(out var referenceId))
                    {
                        ToDoItemSelector.DefaultSelectedItemId = referenceId;
                    }

                    return Result.Success;
                },
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return Item.IfNotNull(nameof(Item))
            .IfSuccessAsync(
                i =>
                    toDoService.UpdateReferenceToDoItemAsync(
                        i.Id,
                        ToDoItemSelector.SelectedItem.ThrowIfNull().Id,
                        ct
                    ),
                ct
            );
    }
}
