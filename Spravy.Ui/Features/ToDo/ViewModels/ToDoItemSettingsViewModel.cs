namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemSettingsViewModel : NavigatableViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IViewFactory viewFactory;

    [ObservableProperty]
    private IApplySettings? settings;

    public ToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        ToDoItemContentViewModel toDoItemContent,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IViewFactory viewFactory
    )
        : base(true)
    {
        ToDoItemContent = toDoItemContent;
        this.toDoService = toDoService;
        this.viewFactory = viewFactory;
        Item = item;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );

        ToDoItemContent.PropertyChanged += OnPropertyChanged;
    }

    public ToDoItemEntityNotify Item { get; }
    public SpravyCommand InitializedCommand { get; }
    public ToDoItemContentViewModel ToDoItemContent { get; }

    public override string ViewId
    {
        get => TypeCache<ToDoItemSettingsViewModel>.Type.Name;
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoService
            .GetToDoItemAsync(Item.Id, ct)
            .IfSuccessAsync(
                toDoItem =>
                    this.PostUiBackground(
                        () =>
                        {
                            ToDoItemContent.Name = toDoItem.Name;
                            ToDoItemContent.Link = toDoItem.Link.TryGetValue(out var uri)
                                ? uri.AbsoluteUri
                                : string.Empty;
                            ToDoItemContent.Type = toDoItem.Type;

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
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

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToDoItemContent.Type))
        {
            Settings = ToDoItemContent.Type switch
            {
                ToDoItemType.Value => viewFactory.CreateValueToDoItemSettingsViewModel(Item),
                ToDoItemType.Planned => viewFactory.CreatePlannedToDoItemSettingsViewModel(Item),
                ToDoItemType.Periodicity
                    => viewFactory.CreatePeriodicityToDoItemSettingsViewModel(Item),
                ToDoItemType.PeriodicityOffset
                    => viewFactory.CreatePeriodicityOffsetToDoItemSettingsViewModel(Item),
                ToDoItemType.Circle => viewFactory.CreateValueToDoItemSettingsViewModel(Item),
                ToDoItemType.Step => viewFactory.CreateValueToDoItemSettingsViewModel(Item),
                ToDoItemType.Group => viewFactory.CreateGroupToDoItemSettingsViewModel(),
                ToDoItemType.Reference
                    => viewFactory.CreateReferenceToDoItemSettingsViewModel(Item),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
