namespace Spravy.Ui.Features.ToDo.Commands;

public class ToDoItemCommands
{
    private readonly IObjectStorage objectStorage;

    public ToDoItemCommands(
        SpravyCommandService spravyCommandService,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        ITaskProgressService taskProgressService
    )
    {
        this.objectStorage = objectStorage;
        NavigateToCurrentToDo = spravyCommandService.NavigateToCurrentToDoItem;

        InitializedCommand =
            SpravyCommand.Create<ToDoItemViewModel>(InitializedAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand InitializedCommand { get; }
    public SpravyCommand NavigateToCurrentToDo { get; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(
        ToDoItemViewModel viewModel,
        CancellationToken ct
    )
    {
        viewModel.FastAddToDoItemViewModel.ParentId = viewModel.Id;

        return objectStorage.GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(viewModel.ViewId, ct)
           .IfSuccessAsync(obj => viewModel.SetStateAsync(obj, ct), ct)
           .IfSuccessAsync(() => viewModel.RefreshAsync(ct), ct);
    }
}