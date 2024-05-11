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

    public void Clear()
    {
        Missed.Clear();
        ReadyForCompleted.Clear();
        Planned.Clear();
        Completed.Clear();
    }

    public void ClearExcept(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        Missed.ClearExcept(ids);
        ReadyForCompleted.ClearExcept(ids);
        Planned.ClearExcept(ids);
        Completed.ClearExcept(ids);
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