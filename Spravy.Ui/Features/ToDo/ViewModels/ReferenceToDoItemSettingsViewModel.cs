namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ReferenceToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private Guid toDoItemId;

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
        return toDoService
            .GetToDoItemAsync(ToDoItemId, ct)
            .IfSuccessAsync(
                item =>
                {
                    ToDoItemSelector.IgnoreIds = new([ToDoItemId,]);

                    if (item.ReferenceId.TryGetValue(out var referenceId))
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
        return toDoService.UpdateReferenceToDoItemAsync(
            ToDoItemId,
            ToDoItemSelector.SelectedItem.ThrowIfNull().Id,
            ct
        );
    }
}
