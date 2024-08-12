namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemsGroupByViewModel : ViewModelBase
{
    [ObservableProperty]
    private GroupBy groupBy = GroupBy.ByStatus;

    [ObservableProperty]
    private object? content;

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
        PropertyChanged += OnPropertyChanged;
    }

    public ToDoItemsGroupByNoneViewModel GroupByNone { get; }
    public ToDoItemsGroupByStatusViewModel GroupByStatus { get; }
    public ToDoItemsGroupByTypeViewModel GroupByType { get; }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return GroupByNone
            .ClearExceptUi(ids)
            .IfSuccess(() => GroupByStatus.ClearExceptUi(ids))
            .IfSuccess(() => GroupByType.ClearExceptUi(ids));
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        return GroupByNone
            .AddOrUpdateUi(item)
            .IfSuccess(() => GroupByStatus.AddOrUpdateUi(item))
            .IfSuccess(() => GroupByType.AddOrUpdateUi(item));
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GroupBy))
        {
            Content = GroupBy switch
            {
                GroupBy.None => GroupByNone,
                GroupBy.ByStatus => GroupByStatus,
                GroupBy.ByType => GroupByType,
                _ => throw new ArgumentOutOfRangeException(nameof(GroupBy), GroupBy, null),
            };
        }
    }
}
