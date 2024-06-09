namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfMonthSelectorViewModel : ViewModelBase, IApplySettings
{
    public ToDoItemDayOfMonthSelectorViewModel()
    {
        SelectedDays = new();
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public AvaloniaList<int> SelectedDays { get; }
    public ICommand InitializedCommand { get; }
    
    [Inject]
    public required IToDoService ToDoService { get; set; }
    
    [Reactive]
    public Guid ToDoItemId { get; set; }
    
    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemMonthlyPeriodicityAsync(ToDoItemId, new(SelectedDays.Select(x => (byte)x)),
            cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetMonthlyPeriodicityAsync(ToDoItemId, cancellationToken)
           .IfSuccessAsync(monthlyPeriodicity => this.InvokeUiBackgroundAsync(() =>
            {
                SelectedDays.AddRange(monthlyPeriodicity.Days.Select(x => (int)x));
                
                return Result.Success;
            }), cancellationToken);
    }
}