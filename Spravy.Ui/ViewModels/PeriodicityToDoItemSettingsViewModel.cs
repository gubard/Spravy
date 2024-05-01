namespace Spravy.Ui.ViewModels;

public class PeriodicityToDoItemSettingsViewModel : ViewModelBase,
    IToDoChildrenTypeProperty,
    IToDoDueDateProperty,
    IToDoTypeOfPeriodicityProperty,
    IIsRequiredCompleteInDueDateProperty,
    IApplySettings
{
    public PeriodicityToDoItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<ToDoItemChildrenType> ChildrenTypes { get; } = new(Enum.GetValues<ToDoItemChildrenType>());
    public AvaloniaList<TypeOfPeriodicity> TypeOfPeriodicities { get; } = new(Enum.GetValues<TypeOfPeriodicity>());
    public ICommand InitializedCommand { get; }

    [Reactive]
    public IApplySettings? Periodicity { get; set; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IKernel Resolve { get; set; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse.IfSuccessAllAsync(cancellationToken,
            () => ToDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken),
            () => ToDoService.UpdateToDoItemDueDateAsync(Id, DueDate, cancellationToken),
            () => ToDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(Id, IsRequiredCompleteInDueDate,
                cancellationToken),
            () => ToDoService.UpdateToDoItemTypeOfPeriodicityAsync(Id, TypeOfPeriodicity, cancellationToken),
            () => Periodicity.ThrowIfNull().ApplySettingsAsync(cancellationToken));
    }

    [Reactive]
    public bool IsRequiredCompleteInDueDate { get; set; }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetPeriodicityToDoItemSettingsAsync(Id, cancellationToken)
           .IfSuccessAsync(setting => this.InvokeUIBackgroundAsync(() =>
            {
                ChildrenType = setting.ChildrenType;
                DueDate = setting.DueDate;
                TypeOfPeriodicity = setting.TypeOfPeriodicity;
                IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;
            }), cancellationToken);
    }

    [Reactive]
    public DateOnly DueDate { get; set; }

    [Reactive]
    public TypeOfPeriodicity TypeOfPeriodicity { get; set; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        this.WhenAnyValue(x => x.TypeOfPeriodicity)
           .Subscribe(x => Periodicity = x switch
            {
                TypeOfPeriodicity.Daily => new EmptyApplySettings(),
                TypeOfPeriodicity.Weekly => Resolve.Get<ToDoItemDayOfWeekSelectorViewModel>()
                   .Case(y => y.ToDoItemId = Id),
                TypeOfPeriodicity.Monthly => Resolve.Get<ToDoItemDayOfMonthSelectorViewModel>()
                   .Case(y => y.ToDoItemId = Id),
                TypeOfPeriodicity.Annually => Resolve.Get<ToDoItemDayOfYearSelectorViewModel>()
                   .Case(y => y.ToDoItemId = Id),
                _ => throw new ArgumentOutOfRangeException(nameof(x), x, null),
            });

        return RefreshAsync(cancellationToken);
    }
}