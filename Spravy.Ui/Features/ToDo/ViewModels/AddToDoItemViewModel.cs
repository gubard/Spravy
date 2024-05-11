namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddToDoItemViewModel : NavigatableViewModelBase
{
    public AddToDoItemViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }
    
    public ICommand InitializedCommand { get; }
    
    [Reactive]
    public object[] Path { get; set; } = Array.Empty<object>();
    
    [Reactive]
    public Guid ParentId { get; set; }
    
    [Inject]
    public required ToDoItemContentViewModel ToDoItemContent { get; init; }
    
    [Inject]
    public required EditDescriptionContentViewModel DescriptionContent { get; init; }
    
    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }
    
    [Inject]
    public required IToDoService ToDoService { get; init; }
    
    public override string ViewId
    {
        get => $"{TypeCache<AddToDoItemViewModel>.Type.Name}:{ParentId}";
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.GetObjectOrDefaultAsync<AddToDoItemViewModelSetting>(ViewId, cancellationToken)
           .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken)
           .IfSuccessAsync(() => ToDoService.GetParentsAsync(ParentId, cancellationToken)
               .IfSuccessAsync(parents =>
                {
                    var path = MaterialIconKind.Home
                       .As<object>()
                       .ToEnumerable()
                       .Concat(parents.ToArray().Select(x => x.Name))
                       .Select(x => x.ThrowIfNull())
                       .ToArray();
                    
                    return this.InvokeUIBackgroundAsync(() => Path = path);
                }, cancellationToken), cancellationToken);
    }
    
    public override Result Stop()
    {
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new AddToDoItemViewModelSetting(this));
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<AddToDoItemViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUIBackgroundAsync(() =>
            {
                ToDoItemContent.Name = s.Name;
                ToDoItemContent.Type = s.Type;
                ToDoItemContent.Link = s.Link;
                DescriptionContent.Description = s.Description;
                DescriptionContent.Type = s.DescriptionType;
            }), cancellationToken);
    }
    
    [ProtoContract]
    private class AddToDoItemViewModelSetting : IViewModelSetting<AddToDoItemViewModelSetting>
    {
        static AddToDoItemViewModelSetting()
        {
            Default = new();
        }
        
        public AddToDoItemViewModelSetting()
        {
        }
        
        public AddToDoItemViewModelSetting(AddToDoItemViewModel viewModel)
        {
            Name = viewModel.ToDoItemContent.Name;
            Type = viewModel.ToDoItemContent.Type;
            Link = viewModel.ToDoItemContent.Link;
            Description = viewModel.DescriptionContent.Description;
            DescriptionType = viewModel.DescriptionContent.Type;
        }
        
        [ProtoMember(1)]
        public string Name { get; set; } = string.Empty;
        
        [ProtoMember(2)]
        public ToDoItemType Type { get; set; }
        
        [ProtoMember(3)]
        public string Link { get; set; } = string.Empty;
        
        [ProtoMember(4)]
        public string Description { get; set; } = string.Empty;
        
        [ProtoMember(5)]
        public DescriptionType DescriptionType { get; set; }
        
        public static AddToDoItemViewModelSetting Default { get; }
    }
}