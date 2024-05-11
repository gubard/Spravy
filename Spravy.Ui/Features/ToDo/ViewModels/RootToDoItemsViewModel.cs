using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RootToDoItemsViewModel : NavigatableViewModelBase, IToDoItemOrderChanger, ITaskProgressServiceProperty
{
    private readonly PageHeaderViewModel pageHeaderViewModel;
    private readonly TaskWork refreshWork;
    private readonly ToDoSubItemsViewModel toDoSubItemsViewModel;
    private readonly FastAddToDoItemViewModel fastAddToDoItemViewModel;
    
    public RootToDoItemsViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    
    public override string ViewId
    {
        get => TypeCache<RootToDoItemsViewModel>.Type.Name;
    }
    
    [Inject]
    public required FastAddToDoItemViewModel FastAddToDoItemViewModel
    {
        get => fastAddToDoItemViewModel;
        [MemberNotNull(nameof(fastAddToDoItemViewModel))]
        init
        {
            fastAddToDoItemViewModel = value;
            fastAddToDoItemViewModel.Refresh = this;
        }
    }
    
    [Inject]
    public required ITaskProgressService TaskProgressService { get; init; }
    
    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel
    {
        get => pageHeaderViewModel;
        [MemberNotNull(nameof(pageHeaderViewModel))]
        init
        {
            pageHeaderViewModel = value;
            pageHeaderViewModel.Header = "Spravy";
            pageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
        }
    }
    
    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel
    {
        get => toDoSubItemsViewModel;
        [MemberNotNull(nameof(toDoSubItemsViewModel))]
        init
        {
            toDoSubItemsViewModel = value;
            
            toDoSubItemsViewModel.List
               .WhenAnyValue(x => x.IsMulti)
               .Skip(1)
               .Subscribe(x =>
                {
                    if (x)
                    {
                        PageHeaderViewModel.SetMultiCommands(ToDoSubItemsViewModel);
                    }
                });
        }
    }
    
    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    [Inject]
    public required IToDoCache ToDoCache { get; init; }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return RefreshCore(cancellationToken).ConfigureAwait(false);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return this.RunProgressAsync(
            () => ObjectStorage.GetObjectOrDefaultAsync<RootToDoItemsViewModelSetting>(ViewId, cancellationToken)
               .IfSuccessAllAsync(cancellationToken, obj => SetStateAsync(obj, cancellationToken),
                    _ => refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false)), cancellationToken);
    }
    
    public async ValueTask<Result> RefreshCore(CancellationToken cancellationToken)
    {
        await refreshWork.RunAsync();
        
        return Result.Success;
    }
    
    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetRootToDoItemIdsAsync(cancellationToken)
           .IfSuccessForEachAsync(id=>ToDoCache.GetToDoItem(id),cancellationToken)
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
        return ObjectStorage.SaveObjectAsync(ViewId, new RootToDoItemsViewModelSetting(this));
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<RootToDoItemsViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUIBackgroundAsync(() =>
            {
                ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;
            }), cancellationToken);
    }
    
    [ProtoContract]
    private class RootToDoItemsViewModelSetting : IViewModelSetting<RootToDoItemsViewModelSetting>
    {
        static RootToDoItemsViewModelSetting()
        {
            Default = new()
            {
                GroupBy = GroupBy.ByStatus,
            };
        }
        
        public RootToDoItemsViewModelSetting(RootToDoItemsViewModel viewModel)
        {
            GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
            IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
        }
        
        public RootToDoItemsViewModelSetting()
        {
        }
        
        [ProtoMember(1)]
        public GroupBy GroupBy { get; set; }
        
        [ProtoMember(2)]
        public bool IsMulti { get; set; }
        
        public static RootToDoItemsViewModelSetting Default { get; }
    }
}