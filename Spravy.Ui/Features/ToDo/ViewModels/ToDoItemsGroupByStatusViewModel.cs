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
    
    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return Missed.ClearExceptUi(items.ToArray().Where(x => x.Status == ToDoItemStatus.Miss).ToArray())
           .IfSuccess(
                () => ReadyForCompleted.ClearExceptUi(items.ToArray()
                   .Where(x => x.Status == ToDoItemStatus.ReadyForComplete)
                   .ToArray()))
           .IfSuccess(
                () => Planned.ClearExceptUi(items.ToArray()
                   .Where(x => x.Status == ToDoItemStatus.Planned)
                   .ToArray()))
           .IfSuccess(
                () => Completed.ClearExceptUi(items.ToArray()
                   .Where(x => x.Status == ToDoItemStatus.Completed)
                   .ToArray()));
    }
    
    public void UpdateItemUi(ToDoItemEntityNotify item)
    {
        switch (item.Status)
        {
            case ToDoItemStatus.Miss:
                Missed.UpdateItemUi(item);
                ReadyForCompleted.RemoveItemUi(item);
                Planned.RemoveItemUi(item);
                Completed.RemoveItemUi(item);
                
                break;
            case ToDoItemStatus.ReadyForComplete:
                Missed.RemoveItemUi(item);
                ReadyForCompleted.UpdateItemUi(item);
                Planned.RemoveItemUi(item);
                Completed.RemoveItemUi(item);
                
                break;
            case ToDoItemStatus.Planned:
                Missed.RemoveItemUi(item);
                ReadyForCompleted.RemoveItemUi(item);
                Planned.UpdateItemUi(item);
                Completed.RemoveItemUi(item);
                
                break;
            case ToDoItemStatus.Completed:
                Missed.RemoveItemUi(item);
                ReadyForCompleted.RemoveItemUi(item);
                Planned.RemoveItemUi(item);
                Completed.UpdateItemUi(item);
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}