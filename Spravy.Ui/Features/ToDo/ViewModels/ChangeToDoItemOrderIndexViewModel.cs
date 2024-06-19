namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ChangeToDoItemOrderIndexViewModel : ViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IToDoCache toDoCache;

    public ChangeToDoItemOrderIndexViewModel(
        IToDoService toDoService,
        IToDoCache toDoCache,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.toDoCache = toDoCache;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand InitializedCommand { get; }
    public Guid Id { get; set; }
    public ReadOnlyMemory<Guid> ChangeToDoItemOrderIndexIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();

    [Reactive]
    public ToDoItemEntityNotify? SelectedItem { get; set; }

    [Reactive]
    public bool IsAfter { get; set; } = true;

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        if (ChangeToDoItemOrderIndexIds.IsEmpty)
        {
            return toDoService.GetSiblingsAsync(Id, cancellationToken)
               .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() =>
                {
                    Items.Clear();

                    return items.ToResult()
                       .IfSuccessForEach(item => toDoCache.UpdateUi(item)
                           .IfSuccess(i =>
                            {
                                Items.Add(i);

                                return Result.Success;
                            }));
                }), cancellationToken);
        }

        return ChangeToDoItemOrderIndexIds.ToResult()
           .IfSuccessForEach(id => toDoCache.GetToDoItem(id))
           .IfSuccessAsync(items => this.InvokeUiBackgroundAsync(() =>
            {
                Items.Clear();
                Items.AddRange(items.ToArray());

                return Result.Success;
            }), cancellationToken);
    }
}