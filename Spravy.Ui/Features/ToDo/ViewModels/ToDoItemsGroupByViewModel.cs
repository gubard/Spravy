namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByViewModel : ViewModelBase
{
    private readonly ToDoItemsGroupByStatusViewModel groupByStatus;
    
    public ToDoItemsGroupByViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Inject]
    public required ToDoItemsGroupByNoneViewModel GroupByNone { get; init; }

    [Inject]
    public required ToDoItemsGroupByStatusViewModel GroupByStatus
    {
        get => groupByStatus;
        [MemberNotNull(nameof(groupByStatus))]
        init
        {
            groupByStatus = value;
            Content = groupByStatus;
        }
    }

    [Inject]
    public required ToDoItemsGroupByTypeViewModel GroupByType { get; init; }
    
    [Reactive]
    public GroupBy GroupBy { get; set; } = GroupBy.ByStatus;

    [Reactive]
    public object? Content { get; set; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        this.WhenAnyValue(x => x.GroupBy)
           .Subscribe(x =>
            {
                Content = x switch
                {
                    GroupBy.None => GroupByNone,
                    GroupBy.ByStatus => GroupByStatus,
                    GroupBy.ByType => GroupByType,
                    _ => throw new ArgumentOutOfRangeException(nameof(x), x, null),
                };
            });

        return Result.AwaitableFalse;
    }

    public void ClearExcept(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        GroupByNone.ClearExcept(ids);
        GroupByStatus.ClearExcept(ids);
        GroupByType.ClearExcept(ids);
    }

    public void UpdateItem(ToDoItemEntityNotify item)
    {
        GroupByNone.UpdateItem(item);
        GroupByStatus.UpdateItem(item);
        GroupByType.UpdateItem(item);
    }
}