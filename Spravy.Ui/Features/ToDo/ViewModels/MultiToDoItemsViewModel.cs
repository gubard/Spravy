namespace Spravy.Ui.Features.ToDo.ViewModels;

public class MultiToDoItemsViewModel : ViewModelBase
{
    private readonly ToDoItemsViewModel favorite;
    
    public MultiToDoItemsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    public AvaloniaList<GroupBy> GroupBys { get; } = new(Enum.GetValues<GroupBy>());
    
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
    public required ToDoItemsGroupByViewModel ToDoItems { get; init; }
    
    [Reactive]
    public GroupBy GroupBy { get; set; } = GroupBy.ByStatus;
    
    [Reactive]
    public bool IsMulti { get; set; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        this.WhenAnyValue(x => x.GroupBy)
           .Subscribe(x =>  ToDoItems.GroupBy = x);
        
        return Result.AwaitableSuccess;
    }
    
    public Result ClearFavoriteExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return Favorite.ClearExceptUi(ids);
    }
    
    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return ToDoItems.ClearExceptUi(ids);
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateFavoriteItemAsync(ToDoItemEntityNotify item)
    {
        return this.InvokeUiBackgroundAsync(() => Favorite.UpdateItemUi(item));
    }
    
    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        return ToDoItems.UpdateItemUi(item);
    }
}