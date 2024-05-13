namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByStatusViewModel : ViewModelBase
{
    private readonly ToDoItemsViewModel completed;
    private readonly ToDoItemsViewModel missed;
    private readonly ToDoItemsViewModel planned;
    private readonly ToDoItemsViewModel readyForCompleted;
    
    [Inject]
    public required ToDoItemsViewModel Missed
    {
        get => missed;
        [MemberNotNull(nameof(missed))]
        init
        {
            missed = value;
            missed.Header = new("ToDoItemsGroupByStatusView.Missed");
        }
    }
    
    [Inject]
    public required ToDoItemsViewModel ReadyForCompleted
    {
        get => readyForCompleted;
        [MemberNotNull(nameof(readyForCompleted))]
        init
        {
            readyForCompleted = value;
            readyForCompleted.Header = new("ToDoItemsGroupByStatusView.ReadyForCompleted");
        }
    }
    
    [Inject]
    public required ToDoItemsViewModel Planned
    {
        get => planned;
        [MemberNotNull(nameof(planned))]
        init
        {
            planned = value;
            planned.Header = new("ToDoItemsGroupByStatusView.Planned");
        }
    }
    
    [Inject]
    public required ToDoItemsViewModel Completed
    {
        get => completed;
        [MemberNotNull(nameof(completed))]
        init
        {
            completed = value;
            completed.Header = new("ToDoItemsGroupByStatusView.Completed");
        }
    }
    
    public ConfiguredValueTaskAwaitable<Result> ClearExceptAsync(
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        CancellationToken cancellationToken
    )
    {
        return Missed.ClearExceptAsync(items.ToArray().Where(x => x.Status == ToDoItemStatus.Miss).ToArray())
           .IfSuccessAsync(
                () => ReadyForCompleted.ClearExceptAsync(items.ToArray()
                   .Where(x => x.Status == ToDoItemStatus.ReadyForComplete)
                   .ToArray()), cancellationToken)
           .IfSuccessAsync(
                () => Planned.ClearExceptAsync(items.ToArray()
                   .Where(x => x.Status == ToDoItemStatus.Planned)
                   .ToArray()), cancellationToken)
           .IfSuccessAsync(
                () => Completed.ClearExceptAsync(items.ToArray()
                   .Where(x => x.Status == ToDoItemStatus.Completed)
                   .ToArray()), cancellationToken);
    }
    
    public void UpdateItem(ToDoItemEntityNotify item)
    {
        switch (item.Status)
        {
            case ToDoItemStatus.Miss:
                Missed.UpdateItem(item);
                ReadyForCompleted.RemoveItem(item);
                Planned.RemoveItem(item);
                Completed.RemoveItem(item);
                
                break;
            case ToDoItemStatus.ReadyForComplete:
                Missed.RemoveItem(item);
                ReadyForCompleted.UpdateItem(item);
                Planned.RemoveItem(item);
                Completed.RemoveItem(item);
                
                break;
            case ToDoItemStatus.Planned:
                Missed.RemoveItem(item);
                ReadyForCompleted.RemoveItem(item);
                Planned.UpdateItem(item);
                Completed.RemoveItem(item);
                
                break;
            case ToDoItemStatus.Completed:
                Missed.RemoveItem(item);
                ReadyForCompleted.RemoveItem(item);
                Planned.RemoveItem(item);
                Completed.UpdateItem(item);
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}