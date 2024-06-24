namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddRootToDoItemViewModel : NavigatableViewModelBase
{
    private readonly IObjectStorage objectStorage;
    
    public AddRootToDoItemViewModel(
        IObjectStorage objectStorage,
        ToDoItemContentViewModel toDoItemContent,
        EditDescriptionContentViewModel descriptionContent,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    ) : base(true)
    {
        this.objectStorage = objectStorage;
        ToDoItemContent = toDoItemContent;
        DescriptionContent = descriptionContent;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);
    }
    
    public SpravyCommand InitializedCommand { get; }
    public ToDoItemContentViewModel ToDoItemContent { get; }
    public EditDescriptionContentViewModel DescriptionContent { get; }
    
    public override string ViewId
    {
        get => TypeCache<AddRootToDoItemViewModel>.Type.Name;
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<AddRootToDoItemViewModelSetting>(ViewId, ct)
           .IfSuccessAsync(obj => SetStateAsync(obj, ct), ct);
    }
    
    public override Result Stop()
    {
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new AddRootToDoItemViewModelSetting(this), ct);
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting.CastObject<AddRootToDoItemViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUiBackgroundAsync(() =>
            {
                ToDoItemContent.Name = s.Name;
                ToDoItemContent.Type = s.Type;
                ToDoItemContent.Link = s.Link;
                DescriptionContent.Description = s.Description;
                DescriptionContent.Type = s.DescriptionType;
                
                return Result.Success;
            }), ct);
    }
    
    [ProtoContract]
    private class AddRootToDoItemViewModelSetting : IViewModelSetting<AddRootToDoItemViewModelSetting>
    {
        static AddRootToDoItemViewModelSetting()
        {
            Default = new();
        }
        
        public AddRootToDoItemViewModelSetting()
        {
        }
        
        public AddRootToDoItemViewModelSetting(AddRootToDoItemViewModel viewModel)
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
        
        public static AddRootToDoItemViewModelSetting Default { get; }
    }
}