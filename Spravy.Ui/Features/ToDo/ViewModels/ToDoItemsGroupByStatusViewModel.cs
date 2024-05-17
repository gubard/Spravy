using Spravy.ToDo.Domain.Errors;

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
           .IfSuccess(() =>
                ReadyForCompleted.ClearExceptUi(items.ToArray()
                   .Where(x => x.Status == ToDoItemStatus.ReadyForComplete)
                   .ToArray()))
           .IfSuccess(() =>
                Planned.ClearExceptUi(items.ToArray().Where(x => x.Status == ToDoItemStatus.Planned).ToArray()))
           .IfSuccess(() =>
                Completed.ClearExceptUi(items.ToArray().Where(x => x.Status == ToDoItemStatus.Completed).ToArray()));
    }
    
    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        switch (item.Status)
        {
            case ToDoItemStatus.Miss:
                return Missed.UpdateItemUi(item)
                   .IfSuccess(() => ReadyForCompleted.RemoveItemUi(item))
                   .IfSuccess(() => Planned.RemoveItemUi(item))
                   .IfSuccess(() => Completed.RemoveItemUi(item));
            case ToDoItemStatus.ReadyForComplete:
                return Missed.RemoveItemUi(item)
                   .IfSuccess(() => ReadyForCompleted.UpdateItemUi(item))
                   .IfSuccess(() => Planned.RemoveItemUi(item))
                   .IfSuccess(() => Completed.RemoveItemUi(item));
            case ToDoItemStatus.Planned:
                return Missed.RemoveItemUi(item)
                   .IfSuccess(() => ReadyForCompleted.RemoveItemUi(item))
                   .IfSuccess(() => Planned.UpdateItemUi(item))
                   .IfSuccess(() => Completed.RemoveItemUi(item));
            case ToDoItemStatus.Completed:
                return Missed.RemoveItemUi(item)
                   .IfSuccess(() => ReadyForCompleted.RemoveItemUi(item))
                   .IfSuccess(() => Planned.RemoveItemUi(item))
                   .IfSuccess(() => Completed.UpdateItemUi(item));
        }
        
        return new(new ToDoItemStatusOutOfRangeError(item.Status));
    }
}