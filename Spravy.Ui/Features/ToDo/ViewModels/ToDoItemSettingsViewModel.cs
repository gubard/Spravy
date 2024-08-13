namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemSettingsViewModel : NavigatableViewModelBase
{
    private readonly IViewFactory viewFactory;

    [ObservableProperty]
    private IApplySettings settings;

    public ToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        ToDoItemContentViewModel toDoItemContent,
        IViewFactory viewFactory
    )
        : base(true)
    {
        Item = item;
        ToDoItemContent = toDoItemContent;
        this.viewFactory = viewFactory;
        ToDoItemContent.Type = Item.Type;
        ToDoItemContent.Name = Item.Name;
        ToDoItemContent.Link = Item.Link;
        settings = CreateSettings();
        ToDoItemContent.PropertyChanged += OnPropertyChanged;
    }

    public ToDoItemEntityNotify Item { get; }
    public ToDoItemContentViewModel ToDoItemContent { get; }

    public override string ViewId
    {
        get => TypeCache<ToDoItemSettingsViewModel>.Type.Name;
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    private IApplySettings CreateSettings()
    {
        return ToDoItemContent.Type switch
        {
            ToDoItemType.Value => viewFactory.CreateValueToDoItemSettingsViewModel(Item),
            ToDoItemType.Group => new EmptyApplySettings(),
            ToDoItemType.Planned => viewFactory.CreatePlannedToDoItemSettingsViewModel(Item),
            ToDoItemType.Periodicity
                => viewFactory.CreatePeriodicityToDoItemSettingsViewModel(Item),
            ToDoItemType.PeriodicityOffset
                => viewFactory.CreatePeriodicityOffsetToDoItemSettingsViewModel(Item),
            ToDoItemType.Circle => viewFactory.CreateValueToDoItemSettingsViewModel(Item),
            ToDoItemType.Step => viewFactory.CreateValueToDoItemSettingsViewModel(Item),
            ToDoItemType.Reference => viewFactory.CreateReferenceToDoItemSettingsViewModel(Item),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToDoItemContent.Type))
        {
            Settings = CreateSettings();
        }
    }
}
