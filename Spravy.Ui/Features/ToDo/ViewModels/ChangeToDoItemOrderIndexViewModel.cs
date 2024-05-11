using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ChangeToDoItemOrderIndexViewModel : ViewModelBase
{
    public ChangeToDoItemOrderIndexViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    
    [Inject]
    public required IMapper Mapper { get; init; }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();
    
    [Reactive]
    public ToDoItemEntityNotify? SelectedItem { get; set; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    [Reactive]
    public bool IsAfter { get; set; } = true;
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetSiblingsAsync(Id, cancellationToken)
           .IfSuccessAsync(
                items => this.InvokeUIBackgroundAsync(() => Items.Clear())
                   .IfSuccessAsync(
                        () => items.ToResult()
                           .IfSuccessForEachAsync(
                                item => ToDoCache.UpdateAsync(item, cancellationToken)
                                   .IfSuccessAsync(i => this.InvokeUIBackgroundAsync(() => Items.Add(i)),
                                        cancellationToken), cancellationToken), cancellationToken), cancellationToken);
    }
}