using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ChangeToDoItemOrderIndexViewModel : ViewModelBase
{
    public ChangeToDoItemOrderIndexViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    public Guid Id { get; set; }
    public ReadOnlyMemory<Guid> ChangeToDoItemOrderIndexIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();
    
    [Reactive]
    public ToDoItemEntityNotify? SelectedItem { get; set; }
    
    [Reactive]
    public bool IsAfter { get; set; } = true;
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        if (ChangeToDoItemOrderIndexIds.IsEmpty)
        {
            return ToDoService.GetSiblingsAsync(Id, cancellationToken)
               .IfSuccessAsync(
                    items => this.InvokeUiBackgroundAsync(() =>
                    {
                        Items.Clear();
                        
                        return items.ToResult()
                           .IfSuccessForEach(item => ToDoCache.UpdateUi(item)
                               .IfSuccess(i =>
                                {
                                    Items.Add(i);
                                    
                                    return Result.Success;
                                }));
                    }), cancellationToken);
        }
        
        return ChangeToDoItemOrderIndexIds.ToResult().IfSuccessForEach(id=>ToDoCache.GetToDoItem(id))
           .IfSuccessAsync(
                items => this.InvokeUiBackgroundAsync(() =>
                {
                    Items.Clear();
                    Items.AddRange(items.ToArray());
                    
                    return Result.Success;
                }), cancellationToken);
    }
}