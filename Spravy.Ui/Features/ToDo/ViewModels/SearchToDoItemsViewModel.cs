namespace Spravy.Ui.Features.ToDo.ViewModels;

public class SearchToDoItemsViewModel : NavigatableViewModelBase, IToDoItemSearchProperties, IToDoItemUpdater
{
    private readonly TaskWork refreshWork;
    
    public SearchToDoItemsViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    
    public override string ViewId
    {
        get => TypeCache<SearchToDoItemsViewModel>.Type.Name;
    }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }
    
    [Reactive]
    public string SearchText { get; set; } = string.Empty;
    
    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.GetObjectOrDefaultAsync<SearchViewModelSetting>(ViewId, cancellationToken)
           .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        return ToDoService.SearchToDoItemIdsAsync(SearchText, cancellationToken)
           .IfSuccessForEachAsync(id => ToDoCache.GetToDoItem(id), cancellationToken)
           .IfSuccessAsync(ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, false, cancellationToken),
                cancellationToken);
    }
    
    public override Result Stop()
    {
        refreshWork.Cancel();
        
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new SearchViewModelSetting(this));
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<SearchViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUiBackgroundAsync(() =>
            {
                SearchText = s.SearchText;
                
                return Result.Success;
            }), cancellationToken);
    }
    
    public Result UpdateInListToDoItemUi(ToDoItemEntityNotify item)
    {
        if (ToDoSubItemsViewModel.List.ToDoItems.GroupByNone.Items.Items.Contains(item))
        {
            return ToDoSubItemsViewModel.List.UpdateItemUi(item);
        }
        
        return Result.Success;
    }
    
    [ProtoContract]
    private class SearchViewModelSetting : IViewModelSetting<SearchViewModelSetting>
    {
        public SearchViewModelSetting(SearchToDoItemsViewModel toDoItemsViewModel)
        {
            SearchText = toDoItemsViewModel.SearchText;
        }
        
        public SearchViewModelSetting()
        {
        }
        
        static SearchViewModelSetting()
        {
            Default = new();
        }
        
        [ProtoMember(1)]
        public string SearchText { get; set; } = string.Empty;
        
        public static SearchViewModelSetting Default { get; }
    }
}