namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByViewModel : ViewModelBase
{
    public ToDoItemsGroupByViewModel(
        ToDoItemsGroupByNoneViewModel groupByNone,
        ToDoItemsGroupByStatusViewModel groupByStatus,
        ToDoItemsGroupByTypeViewModel groupByType
    )
    {
        GroupByNone = groupByNone;
        GroupByStatus = groupByStatus;
        GroupByType = groupByType;
        Content = GroupByStatus;

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

        this.WhenAnyValue(x => x.IsMulti)
            .Subscribe(x =>
            {
                GroupByNone.IsMulti = x;
                GroupByStatus.IsMulti = x;
                GroupByType.IsMulti = x;
            });
    }

    public ToDoItemsGroupByNoneViewModel GroupByNone { get; }
    public ToDoItemsGroupByStatusViewModel GroupByStatus { get; }
    public ToDoItemsGroupByTypeViewModel GroupByType { get; }

    [Reactive]
    public bool IsMulti { get; set; }

    [Reactive]
    public GroupBy GroupBy { get; set; } = GroupBy.ByStatus;

    [Reactive]
    public object? Content { get; set; }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return GroupByNone
            .ClearExceptUi(ids)
            .IfSuccess(() => GroupByStatus.ClearExceptUi(ids))
            .IfSuccess(() => GroupByType.ClearExceptUi(ids));
    }

    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        return GroupByNone
            .UpdateItemUi(item)
            .IfSuccess(() => GroupByStatus.UpdateItemUi(item))
            .IfSuccess(() => GroupByType.UpdateItemUi(item));
    }
}
