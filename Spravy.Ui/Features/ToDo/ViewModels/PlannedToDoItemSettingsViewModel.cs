namespace Spravy.Ui.Features.ToDo.ViewModels;

public class PlannedToDoItemSettingsViewModel : ViewModelBase,
    IToDoChildrenTypeProperty,
    IToDoDueDateProperty,
    IIsRequiredCompleteInDueDateProperty,
    IApplySettings
{
    private ToDoItemChildrenType childrenType;
    private DateOnly dueDate;
    private Guid id;
    private bool isRequiredCompleteInDueDate;

    public PlannedToDoItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<ToDoItemChildrenType> ChildrenTypes { get; } = new(Enum.GetValues<ToDoItemChildrenType>());

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public ICommand InitializedCommand { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse.IfSuccessAllAsync(cancellationToken,
            () => ToDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken),
            () => ToDoService.UpdateToDoItemDueDateAsync(Id, DueDate, cancellationToken),
            () => ToDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(Id, IsRequiredCompleteInDueDate,
                cancellationToken));
    }

    public bool IsRequiredCompleteInDueDate
    {
        get => isRequiredCompleteInDueDate;
        set => this.RaiseAndSetIfChanged(ref isRequiredCompleteInDueDate, value);
    }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public ToDoItemChildrenType ChildrenType
    {
        get => childrenType;
        set => this.RaiseAndSetIfChanged(ref childrenType, value);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetPlannedToDoItemSettingsAsync(Id, cancellationToken)
           .IfSuccessAsync(setting => this.InvokeUIBackgroundAsync(() =>
            {
                ChildrenType = setting.ChildrenType;
                DueDate = setting.DueDate;
                IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;
            }), cancellationToken);
    }

    public DateOnly DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }
}