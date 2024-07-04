namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemSettingsViewModel : NavigatableViewModelBase
{
    private readonly IToDoService toDoService;
    private readonly IServiceFactory serviceFactory;

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

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        ToDoItemContent
            .WhenAnyValue(x => x.Type)
            .Subscribe(x =>
                Settings = x switch
                {
                    ToDoItemType.Value
                        => serviceFactory
                            .CreateService<ValueToDoItemSettingsViewModel>()
                            .Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.Planned
                        => serviceFactory
                            .CreateService<PlannedToDoItemSettingsViewModel>()
                            .Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.Periodicity
                        => serviceFactory
                            .CreateService<PeriodicityToDoItemSettingsViewModel>()
                            .Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.PeriodicityOffset
                        => serviceFactory
                            .CreateService<PeriodicityOffsetToDoItemSettingsViewModel>()
                            .Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.Circle
                        => serviceFactory
                            .CreateService<ValueToDoItemSettingsViewModel>()
                            .Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.Step
                        => serviceFactory
                            .CreateService<ValueToDoItemSettingsViewModel>()
                            .Case(vm => vm.Id = ToDoItemId),
                    ToDoItemType.Group
                        => serviceFactory.CreateService<GroupToDoItemSettingsViewModel>(),
                    ToDoItemType.Reference
                        => serviceFactory
                            .CreateService<ReferenceToDoItemSettingsViewModel>()
                            .Case(vm => vm.ToDoItemId = ToDoItemId),
                    _ => throw new ArgumentOutOfRangeException(),
                }
            );

        return RefreshAsync(ct);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoService
            .GetToDoItemAsync(ToDoItemId, ct)
            .IfSuccessAsync(
                toDoItem =>
                    this.PostUiBackground(() =>
                    {
                        ToDoItemContent.Name = toDoItem.Name;
                        ToDoItemContent.Link = toDoItem.Link.TryGetValue(out var uri)
                            ? uri.AbsoluteUri
                            : string.Empty;
                        ToDoItemContent.Type = toDoItem.Type;

                        return Result.Success;
                    }),
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
}
