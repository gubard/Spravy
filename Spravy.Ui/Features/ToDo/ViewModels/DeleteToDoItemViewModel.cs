namespace Spravy.Ui.Features.ToDo.ViewModels;

public class DeleteToDoItemViewModel : ViewModelBase
{
    public DeleteToDoItemViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    public ReadOnlyMemory<Guid> DeletedIds { get; set; } = ReadOnlyMemory<Guid>.Empty;
    
    public string Name
    {
        get
        {
            if (DeletedIds.IsEmpty)
            {
                return Item?.Name ?? string.Empty;
            }
            
            return string.Join(", ", DeletedIds.Select(x => ToDoCache.GetToDoItem(x).ThrowIfError().Name).ToArray());
        }
    }
    
    [Reactive]
    public ToDoItemEntityNotify? Item { get; set; }
    
    [Reactive]
    public string ChildrenText { get; set; } = string.Empty;
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        var statuses = Enum.GetValues<ToDoItemStatus>();
        
        return Item.IfNotNull(nameof(Item))
           .IfSuccessAsync(item => Result.AwaitableFalse.IfSuccessAllAsync(cancellationToken,
                () => ToDoService.GetToDoItemAsync(item.Id, cancellationToken)
                   .IfSuccessAsync(i => this.InvokeUiBackgroundAsync(() => ToDoCache.UpdateUi(i)), cancellationToken)
                   .ToResultOnlyAsync(),
                () => ToDoService.GetParentsAsync(item.Id, cancellationToken)
                   .IfSuccessAsync(
                        parents => this.InvokeUiBackgroundAsync(() => ToDoCache.UpdateParentsUi(item.Id, parents)),
                        cancellationToken), () =>
                {
                    if (DeletedIds.IsEmpty)
                    {
                        return ToDoService.ToDoItemToStringAsync(new(statuses, item.Id), cancellationToken)
                           .IfSuccessAsync(text => this.InvokeUiBackgroundAsync(() =>
                            {
                                ChildrenText = text;
                                
                                return Result.Success;
                            }), cancellationToken);
                    }
                    
                    return DeletedIds.ToResult()
                       .IfSuccessForEachAsync(
                            id => ToDoService.ToDoItemToStringAsync(new(statuses, id), cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(values =>
                        {
                            var childrenText = string.Join(Environment.NewLine, values.ToArray());
                            
                            return this.InvokeUiBackgroundAsync(() =>
                            {
                                ChildrenText = childrenText;
                                
                                return Result.Success;
                            });
                        }, cancellationToken);
                }), cancellationToken);
    }
}