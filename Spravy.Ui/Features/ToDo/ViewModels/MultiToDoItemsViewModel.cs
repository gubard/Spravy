namespace Spravy.Ui.Features.ToDo.ViewModels;

public class MultiToDoItemsViewModel : ViewModelBase
{
    public MultiToDoItemsViewModel(
        ToDoItemsViewModel favorite,
        ToDoItemsGroupByViewModel toDoItems,
        IErrorHandler errorHandler
    )
    {
        GroupBy = GroupBy.ByStatus;
        favorite.Header = new("MultiToDoItemsView.Favorite");
        Favorite = favorite;
        ToDoItems = toDoItems;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler);
        
        this.WhenAnyValue(x => x.IsMulti)
           .Subscribe(x =>
            {
                Favorite.IsMulti = x;
                ToDoItems.IsMulti = x;
            });
    }
    
    public SpravyCommand InitializedCommand { get; }
    public ToDoItemsViewModel Favorite { get; }
    public ToDoItemsGroupByViewModel ToDoItems { get; }
    
    [Reactive]
    public GroupBy GroupBy { get; set; }
    
    [Reactive]
    public bool IsMulti { get; set; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        this.WhenAnyValue(x => x.GroupBy).Subscribe(x => ToDoItems.GroupBy = x);
        
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