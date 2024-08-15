namespace Spravy.Ui.Features.ToDo.Commands;

public class ToDoItemCommands
{
    public ToDoItemCommands(IErrorHandler errorHandler, ITaskProgressService taskProgressService)
    {
        InitializedCommand = SpravyCommand.Create<ToDoItemViewModel>(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }

    private Cvtar InitializedAsync(ToDoItemViewModel viewModel, CancellationToken ct)
    {
        return viewModel.RefreshAsync(ct);
    }
}
