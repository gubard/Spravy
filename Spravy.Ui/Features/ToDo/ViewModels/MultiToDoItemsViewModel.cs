namespace Spravy.Ui.Features.ToDo.ViewModels;

public class MultiToDoItemsViewModel : ViewModelBase
{
    private readonly ToDoItemsViewModel favorite;
    private readonly ToDoItemsGroupByViewModel multiToDoItems;
    private readonly ToDoItemsGroupByViewModel toDoItems;
    
    public MultiToDoItemsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    public AvaloniaList<GroupBy> GroupBys { get; } = new(Enum.GetValues<GroupBy>());
    
    [Inject]
    public required IMapper Mapper { get; init; }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required ToDoItemsViewModel Favorite
    {
        get => favorite;
        [MemberNotNull(nameof(favorite))]
        init
        {
            favorite = value;
            favorite.Header = new("MultiToDoItemsView.Favorite");
        }
    }
    
    [Inject]
    public required ToDoItemsGroupByViewModel ToDoItems
    {
        get => toDoItems;
        [MemberNotNull(nameof(toDoItems))]
        init
        {
            toDoItems = value;
            Content = toDoItems;
        }
    }
    
    [Inject]
    public required ToDoItemsGroupByViewModel MultiToDoItems
    {
        get => multiToDoItems;
        [MemberNotNull(nameof(multiToDoItems))]
        init
        {
            multiToDoItems = value;
            
            multiToDoItems.GroupByNone.Items.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByNone.Items.Items));
            
            multiToDoItems.GroupByStatus.Missed.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByStatus.Missed.Items));
            
            multiToDoItems.GroupByStatus.Completed.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByStatus.Completed.Items));
            
            multiToDoItems.GroupByStatus.Planned.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByStatus.Planned.Items));
            
            multiToDoItems.GroupByStatus.ReadyForCompleted.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByStatus.ReadyForCompleted.Items));
            
            multiToDoItems.GroupByType.Groups.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Groups.Items));
            
            multiToDoItems.GroupByType.Circles.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Circles.Items));
            
            multiToDoItems.GroupByType.Periodicitys.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Periodicitys.Items));
            
            multiToDoItems.GroupByType.Planneds.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Planneds.Items));
            
            multiToDoItems.GroupByType.Steps.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Steps.Items));
            
            multiToDoItems.GroupByType.Values.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Values.Items));
            
            multiToDoItems.GroupByType.PeriodicityOffsets.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.PeriodicityOffsets.Items));
        }
    }
    
    [Reactive]
    public GroupBy GroupBy { get; set; } = GroupBy.ByStatus;
    
    [Reactive]
    public bool IsMulti { get; set; }
    
    [Reactive]
    public object? Content { get; set; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        this.WhenAnyValue(x => x.IsMulti).Subscribe(x => Content = x ? MultiToDoItems : ToDoItems);
        
        this.WhenAnyValue(x => x.GroupBy)
           .Subscribe(x =>
            {
                ToDoItems.GroupBy = x;
                MultiToDoItems.GroupBy = x;
            });
        
        return Result.AwaitableFalse;
    }
    
    public Result ClearFavoriteExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return Favorite.ClearExceptUi(ids);
    }
    
    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return ToDoItems.ClearExceptUi(ids)
           .IfSuccess(() => MultiToDoItems.ClearExceptUi(ids));
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateFavoriteItemAsync(ToDoItemEntityNotify item)
    {
        return this.InvokeUiBackgroundAsync(() => Favorite.UpdateItemUi(item));
    }
    
    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        ToDoItems.UpdateItemUi(item);
        MultiToDoItems.UpdateItemUi(item);
        
        return Result.Success;
    }
}