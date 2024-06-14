namespace Spravy.Ui.Features.ToDo.Commands;

public class ToDoItemCommands
{
    private readonly IObjectStorage objectStorage;
    private readonly ITaskProgressService taskProgressService;
    
    public ToDoItemCommands(
        IToDoService toDoService,
        INavigator navigator,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        ITaskProgressService taskProgressService
    )
    {
        this.objectStorage = objectStorage;
        this.taskProgressService = taskProgressService;
        NavigateToCurrentToDo = SpravyCommand.CreateNavigateToCurrentToDoItem(toDoService, navigator, taskProgressService, errorHandler);
        InitializedCommand = SpravyCommand.Create<ToDoItemViewModel>(InitializedAsync, errorHandler);
    }
    
    public SpravyCommand InitializedCommand { get; }
    public SpravyCommand NavigateToCurrentToDo { get; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(
        ToDoItemViewModel viewModel,
        CancellationToken cancellationToken
    )
    {
        viewModel.FastAddToDoItemViewModel.ParentId = viewModel.Id;
        
        return taskProgressService.RunProgressAsync(
            () => objectStorage.GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(viewModel.ViewId, cancellationToken)
               .IfSuccessAsync(obj => viewModel.SetStateAsync(obj, cancellationToken), cancellationToken)
               .IfSuccessAsync(() => viewModel.RefreshAsync(cancellationToken), cancellationToken), cancellationToken);
    }
}