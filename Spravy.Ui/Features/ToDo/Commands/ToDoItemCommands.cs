namespace Spravy.Ui.Features.ToDo.Commands;

public class ToDoItemCommands
{
    private readonly IObjectStorage objectStorage;

    public ToDoItemCommands(
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        ITaskProgressService taskProgressService
    )
    {
        this.objectStorage = objectStorage;

        InitializedCommand = SpravyCommand.Create<ToDoItemViewModel>(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(
        ToDoItemViewModel viewModel,
        CancellationToken ct
    )
    {
        return objectStorage
            .GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(viewModel.ViewId, ct)
            .IfSuccessAsync(obj => viewModel.SetStateAsync(obj, ct), ct)
            .IfSuccessAsync(() => viewModel.RefreshAsync(ct), ct);
    }
}
