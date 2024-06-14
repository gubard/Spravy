namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemSettingsViewModel : NavigatableViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IKernel resolver;
    
    public ToDoItemSettingsViewModel(
        ToDoItemContentViewModel toDoItemContent,
        IKernel resolver,
        IToDoService toDoService,
        IErrorHandler errorHandler
    ) : base(true)
    {
        ToDoItemContent = toDoItemContent;
        this.resolver = resolver;
        this.toDoService = toDoService;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler);
    }
    
    public override string ViewId
    {
        get => TypeCache<ToDoItemSettingsViewModel>.Type.Name;
    }
    
    public SpravyCommand InitializedCommand { get; }
    public ToDoItemContentViewModel ToDoItemContent { get; }
    
    [Reactive]
    public IApplySettings? Settings { get; set; }
    
    [Reactive]
    public Guid ToDoItemId { get; set; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        ToDoItemContent.WhenAnyValue(x => x.Type)
           .Subscribe(x => Settings = x switch
            {
                ToDoItemType.Value => resolver.Get<ValueToDoItemSettingsViewModel>().Case(vm => vm.Id = ToDoItemId),
                ToDoItemType.Planned => resolver.Get<PlannedToDoItemSettingsViewModel>().Case(vm => vm.Id = ToDoItemId),
                ToDoItemType.Periodicity => resolver.Get<PeriodicityToDoItemSettingsViewModel>()
                   .Case(vm => vm.Id = ToDoItemId),
                ToDoItemType.PeriodicityOffset => resolver.Get<PeriodicityOffsetToDoItemSettingsViewModel>()
                   .Case(vm => vm.Id = ToDoItemId),
                ToDoItemType.Circle => resolver.Get<ValueToDoItemSettingsViewModel>().Case(vm => vm.Id = ToDoItemId),
                ToDoItemType.Step => resolver.Get<ValueToDoItemSettingsViewModel>().Case(vm => vm.Id = ToDoItemId),
                ToDoItemType.Group => resolver.Get<GroupToDoItemSettingsViewModel>(),
                ToDoItemType.Reference => resolver.Get<ReferenceToDoItemSettingsViewModel>()
                   .Case(vm => vm.ToDoItemId = ToDoItemId),
                _ => throw new ArgumentOutOfRangeException(),
            });
        
        return RefreshAsync(cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return toDoService.GetToDoItemAsync(ToDoItemId, cancellationToken)
           .IfSuccessAsync(toDoItem => this.InvokeUiBackgroundAsync(() =>
            {
                ToDoItemContent.Name = toDoItem.Name;
                ToDoItemContent.Link = toDoItem.Link.Value?.AbsoluteUri ?? string.Empty;
                ToDoItemContent.Type = toDoItem.Type;
                
                return Result.Success;
            }), cancellationToken);
    }
    
    public override Result Stop()
    {
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableSuccess;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableSuccess;
    }
}