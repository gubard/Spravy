namespace Spravy.Ui.ViewModels;

public class ValueToDoItemSettingsViewModel : ViewModelBase, IToDoChildrenTypeProperty, IApplySettings
{
    public ValueToDoItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<ToDoItemChildrenType> ChildrenTypes { get; } = new(Enum.GetValues<ToDoItemChildrenType>());

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public ICommand InitializedCommand { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken);
    }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetValueToDoItemSettingsAsync(Id, cancellationToken)
           .IfSuccessAsync(setting => this.InvokeUIBackgroundAsync(() => ChildrenType = setting.ChildrenType),
                cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }
}