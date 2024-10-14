namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemSettingsViewModel : DialogableViewModelBase, IApplySettings
{
    private readonly IViewFactory viewFactory;
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private IEditToDoItems edit;

    public ToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        ToDoItemContentViewModel toDoItemContent,
        IViewFactory viewFactory,
        IToDoService toDoService
    )
    {
        Item = item;
        ToDoItemContent = toDoItemContent;
        this.viewFactory = viewFactory;
        this.toDoService = toDoService;
        ToDoItemContent.Type = Item.Type;
        ToDoItemContent.Name = Item.Name;
        ToDoItemContent.Link = Item.Link;
        edit = CreateEdit();
        ToDoItemContent.PropertyChanged += OnPropertyChanged;
    }

    public ToDoItemEntityNotify Item { get; }
    public ToDoItemContentViewModel ToDoItemContent { get; }

    public override string ViewId
    {
        get => TypeCache<ToDoItemSettingsViewModel>.Type.Name;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    private IEditToDoItems CreateEdit()
    {
        return ToDoItemContent.Type switch
        {
            ToDoItemType.Value => viewFactory.CreateValueToDoItemSettingsViewModel(Item),
            ToDoItemType.Group => new EmptyEditToDoItems(),
            ToDoItemType.Planned => viewFactory.CreatePlannedToDoItemSettingsViewModel(Item),
            ToDoItemType.Periodicity
                => viewFactory.CreatePeriodicityToDoItemSettingsViewModel(Item),
            ToDoItemType.PeriodicityOffset
                => viewFactory.CreatePeriodicityOffsetToDoItemSettingsViewModel(Item),
            ToDoItemType.Circle => viewFactory.CreateValueToDoItemSettingsViewModel(Item),
            ToDoItemType.Step => viewFactory.CreateValueToDoItemSettingsViewModel(Item),
            ToDoItemType.Reference => viewFactory.CreateReferenceToDoItemSettingsViewModel(Item),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToDoItemContent.Type))
        {
            Edit = CreateEdit();
        }
    }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.EditToDoItemsAsync(
            Edit.GetEditToDoItems()
                .SetType(new(ToDoItemContent.Type))
                .SetName(new(ToDoItemContent.Name))
                .SetLink(new(ToDoItemContent.Link.ToOptionUri())),
            ct
        );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}
