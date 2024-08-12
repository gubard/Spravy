namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemSettingsViewModel : NavigatableViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IServiceFactory serviceFactory;
    private readonly IViewFactory viewFactory;

    [ObservableProperty]
    private IApplySettings? settings;

    public ToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        ToDoItemContentViewModel toDoItemContent,
        IServiceFactory serviceFactory,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IViewFactory viewFactory
    )
        : base(true)
    {
        ToDoItemContent = toDoItemContent;
        this.serviceFactory = serviceFactory;
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
        return Item.IfNotNull(nameof(Item))
            .IfSuccessAsync(i => toDoService.GetToDoItemAsync(i.Id, ct), ct)
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
            if (Item is null)
            {
                return;
            }

            Settings = ToDoItemContent.Type switch
            {
                ToDoItemType.Value
                    => serviceFactory
                        .CreateService<ValueToDoItemSettingsViewModel>()
                        .Case(vm => vm.Id = Item.Id),
                ToDoItemType.Planned
                    => serviceFactory
                        .CreateService<PlannedToDoItemSettingsViewModel>()
                        .Case(vm => vm.Item = Item),
                ToDoItemType.Periodicity
                    => serviceFactory
                        .CreateService<PeriodicityToDoItemSettingsViewModel>()
                        .Case(vm => vm.Item = Item),
                ToDoItemType.PeriodicityOffset
                    => serviceFactory
                        .CreateService<PeriodicityOffsetToDoItemSettingsViewModel>()
                        .Case(vm => vm.Item = Item),
                ToDoItemType.Circle
                    => serviceFactory
                        .CreateService<ValueToDoItemSettingsViewModel>()
                        .Case(vm => vm.Id = Item.Id),
                ToDoItemType.Step
                    => serviceFactory
                        .CreateService<ValueToDoItemSettingsViewModel>()
                        .Case(vm => vm.Id = Item.Id),
                ToDoItemType.Group
                    => serviceFactory.CreateService<GroupToDoItemSettingsViewModel>(),
                ToDoItemType.Reference
                    => viewFactory.CreateReferenceToDoItemSettingsViewModel(Item),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
