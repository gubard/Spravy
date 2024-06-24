namespace Spravy.Ui.Features.ToDo.ViewModels;

public class PeriodicityToDoItemSettingsViewModel : ViewModelBase,
    IToDoChildrenTypeProperty,
    IToDoDueDateProperty,
    IToDoTypeOfPeriodicityProperty,
    IIsRequiredCompleteInDueDateProperty,
    IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IServiceFactory serviceFactory;
    
    public PeriodicityToDoItemSettingsViewModel(
        IToDoService toDoService,
        IServiceFactory serviceFactory,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.serviceFactory = serviceFactory;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);
    }
    
    public SpravyCommand InitializedCommand { get; }
    
    [Reactive]
    public IApplySettings? Periodicity { get; set; }
    
    [Reactive]
    public bool IsRequiredCompleteInDueDate { get; set; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public ToDoItemChildrenType ChildrenType { get; set; }
    
    [Reactive]
    public DateOnly DueDate { get; set; }
    
    [Reactive]
    public TypeOfPeriodicity TypeOfPeriodicity { get; set; }
    
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, ct)
           .IfSuccessAsync(() => toDoService.UpdateToDoItemDueDateAsync(Id, DueDate, ct),
                ct)
           .IfSuccessAsync(
                () => toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(Id, IsRequiredCompleteInDueDate,
                    ct), ct)
           .IfSuccessAsync(
                () => toDoService.UpdateToDoItemTypeOfPeriodicityAsync(Id, TypeOfPeriodicity, ct),
                ct)
           .IfSuccessAsync(() => Periodicity.ThrowIfNull().ApplySettingsAsync(ct), ct);
    }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoService.GetPeriodicityToDoItemSettingsAsync(Id, ct)
           .IfSuccessAsync(setting => this.InvokeUiBackgroundAsync(() =>
            {
                ChildrenType = setting.ChildrenType;
                DueDate = setting.DueDate;
                TypeOfPeriodicity = setting.TypeOfPeriodicity;
                IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;
                
                return Result.Success;
            }), ct);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        this.WhenAnyValue(x => x.TypeOfPeriodicity)
           .Subscribe(x => Periodicity = x switch
            {
                TypeOfPeriodicity.Daily => new EmptyApplySettings(),
                TypeOfPeriodicity.Weekly => serviceFactory.CreateService<ToDoItemDayOfWeekSelectorViewModel>()
                   .Case(y => y.ToDoItemId = Id),
                TypeOfPeriodicity.Monthly => serviceFactory.CreateService<ToDoItemDayOfMonthSelectorViewModel>()
                   .Case(y => y.ToDoItemId = Id),
                TypeOfPeriodicity.Annually => serviceFactory.CreateService<ToDoItemDayOfYearSelectorViewModel>()
                   .Case(y => y.ToDoItemId = Id),
                _ => throw new ArgumentOutOfRangeException(nameof(x), x, null),
            });
        
        return RefreshAsync(ct);
    }
}