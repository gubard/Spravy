using Spravy.Ui.Features.ToDo.Settings;

namespace Spravy.Ui.Features.ToDo.Commands;

public class ToDoItemCommands : ITaskProgressServiceProperty
{
    private readonly IObjectStorage objectStorage;
    
    public ToDoItemCommands(
        IToDoService toDoService,
        INavigator navigator,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        ITaskProgressService taskProgressService
    )
    {
        this.objectStorage = objectStorage;
        TaskProgressService = taskProgressService;
        NavigateToCurrentToDo = SpravyCommand.CreateNavigateToCurrentToDoItem(toDoService, navigator, errorHandler);
        InitializedCommand = SpravyCommand.Create<ToDoItemViewModel>(InitializedAsync, errorHandler);
    }
    
    public SpravyCommand InitializedCommand { get; }
    public SpravyCommand NavigateToCurrentToDo { get; }
    public ITaskProgressService TaskProgressService { get; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(
        ToDoItemViewModel viewModel,
        CancellationToken cancellationToken
    )
    {
        return this.RunProgressAsync(
            () => objectStorage.GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(viewModel.ViewId, cancellationToken)
               .IfSuccessAsync(obj => viewModel.SetStateAsync(obj, cancellationToken), cancellationToken)
               .IfSuccessAsync(() => viewModel.RefreshAsync(cancellationToken), cancellationToken), cancellationToken);
    }
}