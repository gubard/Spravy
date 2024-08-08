namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemSettingsViewModel : NavigatableViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IServiceFactory serviceFactory;

    [ObservableProperty]
    private IApplySettings? settings;

    [ObservableProperty]
    private ToDoItemEntityNotify? item;

    public ToDoItemSettingsViewModel(
        ToDoItemContentViewModel toDoItemContent,
        IServiceFactory serviceFactory,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
        : base(true)
    {
        ToDoItemContent = toDoItemContent;
        this.serviceFactory = serviceFactory;
        this.toDoService = toDoService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );

        ToDoItemContent.PropertyChanged += OnPropertyChanged;
    }

    public override string ViewId
    {
        get => TypeCache<ToDoItemSettingsViewModel>.Type.Name;
    }

    public SpravyCommand InitializedCommand { get; }
    public ToDoItemContentViewModel ToDoItemContent { get; }

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
                        .Case(vm => vm.Id = Item.Id),
                ToDoItemType.Periodicity
                    => serviceFactory
                        .CreateService<PeriodicityToDoItemSettingsViewModel>()
                        .Case(vm => vm.Id = Item.Id),
                ToDoItemType.PeriodicityOffset
                    => serviceFactory
                        .CreateService<PeriodicityOffsetToDoItemSettingsViewModel>()
                        .Case(vm => vm.Id = Item.Id),
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
                    => serviceFactory
                        .CreateService<ReferenceToDoItemSettingsViewModel>()
                        .Case(vm => vm.Item = Item),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
