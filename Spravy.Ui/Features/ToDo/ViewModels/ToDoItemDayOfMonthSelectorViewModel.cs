namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfMonthSelectorViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    
    public ToDoItemDayOfMonthSelectorViewModel(IToDoService toDoService, IErrorHandler errorHandler)
    {
        this.toDoService = toDoService;
        SelectedDays = new();
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler);
    }
    
    public AvaloniaList<int> SelectedDays { get; }
    public SpravyCommand InitializedCommand { get; }
    
    [Reactive]
    public Guid ToDoItemId { get; set; }
    
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return toDoService.UpdateToDoItemMonthlyPeriodicityAsync(ToDoItemId, new(SelectedDays.Select(x => (byte)x)),
            cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetMonthlyPeriodicityAsync(ToDoItemId, cancellationToken)
           .IfSuccessAsync(monthlyPeriodicity => this.InvokeUiBackgroundAsync(() =>
            {
                SelectedDays.AddRange(monthlyPeriodicity.Days.Select(x => (int)x));
                
                return Result.Success;
            }), cancellationToken);
    }
}