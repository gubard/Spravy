namespace Spravy.Ui.Features.ToDo.ViewModels;

public class DeleteToDoItemViewModel : ViewModelBase
{
    public DeleteToDoItemViewModel(IToDoService toDoService, IToDoCache toDoCache, IErrorHandler errorHandler)
    {
        DeleteItems = new();
        
        InitializedCommand = SpravyCommand.Create(cancellationToken =>
        {
            var statuses = Enum.GetValues<ToDoItemStatus>();
            
            return Result.AwaitableSuccess.IfSuccessAllAsync(cancellationToken, () =>
            {
                if (Item is null)
                {
                    return Result.AwaitableSuccess;
                }
                
                return toDoService.GetToDoItemAsync(Item.Id, cancellationToken)
                   .IfSuccessAsync(i => this.InvokeUiBackgroundAsync(() => toDoCache.UpdateUi(i)), cancellationToken)
                   .ToResultOnlyAsync();
            }, () =>
            {
                if (Item is null)
                {
                    return Result.AwaitableSuccess;
                }
                
                return toDoService.GetParentsAsync(Item.Id, cancellationToken)
                   .IfSuccessAsync(
                        parents => this.InvokeUiBackgroundAsync(() => toDoCache.UpdateParentsUi(Item.Id, parents)),
                        cancellationToken);
            }, () =>
            {
                if (DeleteItems.IsEmpty())
                {
                    return Item.IfNotNull(nameof(Item))
                       .IfSuccessAsync(item => toDoService
                           .ToDoItemToStringAsync(new(statuses, item.Id), cancellationToken)
                           .IfSuccessAsync(text => this.InvokeUiBackgroundAsync(() =>
                            {
                                ChildrenText = text;
                                
                                return Result.Success;
                            }), cancellationToken), cancellationToken);
                }
                
                return DeleteItems.ToReadOnlyMemory()
                   .ToResult()
                   .IfSuccessForEachAsync(
                        i => toDoService.ToDoItemToStringAsync(new(statuses, i.Id), cancellationToken).IfSuccessAsync(
                            str => $"{i.Name}{Environment.NewLine} {str.Split(Environment.NewLine).JoinString($"{Environment.NewLine} ")}".ToResult(), cancellationToken),
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
            });
        }, errorHandler);
    }
    
    public SpravyCommand InitializedCommand { get; }
    public AvaloniaList<ToDoItemEntityNotify> DeleteItems { get; }
    
    [Reactive]
    public ToDoItemEntityNotify? Item { get; set; }
    
    [Reactive]
    public string ChildrenText { get; set; } = string.Empty;
}