namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByViewModel : ViewModelBase
{
    public ToDoItemsGroupByViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    
    [Inject]
    public required ToDoItemsGroupByNoneViewModel GroupByNone { get; init; }
    
    [Inject]
    public required ToDoItemsGroupByStatusViewModel GroupByStatus { get; init; }
    
    [Inject]
    public required ToDoItemsGroupByTypeViewModel GroupByType { get; init; }
    
    [Reactive]
    public GroupBy GroupBy { get; set; } = GroupBy.ByStatus;
    
    [Reactive]
    public object? Content { get; set; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
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
        
        return Result.AwaitableSuccess;
    }
    
    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return GroupByNone.ClearExceptUi(ids)
           .IfSuccess(() => GroupByStatus.ClearExceptUi(ids))
           .IfSuccess(() => GroupByType.ClearExceptUi(ids));
    }
    
    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        return GroupByNone.UpdateItemUi(item)
           .IfSuccess(() => GroupByStatus.UpdateItemUi(item))
           .IfSuccess(() => GroupByType.UpdateItemUi(item));
    }
}