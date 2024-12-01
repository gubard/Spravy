namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemCreateTimerViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IObjectStorage objectStorage;
    private readonly IScheduleService scheduleService;
    private readonly ISerializer serializer;

    [ObservableProperty]
    private DateTime date = DateTime.Today;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private TimeSpan time = DateTime.Now.TimeOfDay;

    public ToDoItemCreateTimerViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IObjectStorage objectStorage,
        ISerializer serializer,
        IScheduleService scheduleService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    ) : base(editItem, editItems)
    {
        this.objectStorage = objectStorage;
        this.serializer = serializer;
        this.scheduleService = scheduleService;

        AddTime = SpravyCommand.Create<TimeSpan>(
            (t, ct) => this.PostUiBackground(
                    () =>
                    {
                        Time += t;

                        return Result.Success;
                    },
                    ct
                )
               .ToValueTaskResult()
               .ConfigureAwait(false),
            errorHandler,
            taskProgressService
        );

        if (editItem.TryGetValue(out var item))
        {
            name = item.Name;
        }
    }

    public SpravyCommand AddTime { get; }
    public AvaloniaList<TimeSpan> Times { get; } = new();
    public AvaloniaList<string> Names { get; } = new();

    public override string ViewId =>
        EditItem.TryGetValue(out var editItem)
            ? $"{TypeCache<ToDoItemCreateTimerViewModel>.Type.Name}:{editItem.Id}"
            : $"{TypeCache<ToDoItemCreateTimerViewModel>.Type.Name}";

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return CreateAddTimerParametersAsync(ct)
           .IfSuccessAsync(parameters => scheduleService.AddTimerAsync(parameters, ct), ct);
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<ToDoItemCreateTimerViewModelSettings>(ViewId, ct)
           .IfSuccessAsync(
                setting => this.PostUiBackground(
                    () =>
                    {
                        Names.UpdateUi(setting.Names);
                        Times.UpdateUi(setting.Times);

                        return Result.Success;
                    },
                    ct
                ),
                ct
            );
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new ToDoItemCreateTimerViewModelSettings(this), ct);
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<AddTimerParameters>>> CreateAddTimerParametersAsync(
        CancellationToken ct
    )
    {
        return ResultCurrentIds.ToResult()
           .IfSuccessForEachAsync(
                id => serializer.SerializeAsync(
                        new AddToDoItemToFavoriteEventOptions
                        {
                            ToDoItemId = id,
                        },
                        ct
                    )
                   .IfSuccessAsync(
                        content => new AddTimerParameters(
                            Date.Date.Add(Time),
                            AddToDoItemToFavoriteEventOptions.EventId,
                            content,
                            Name
                        ).ToResult(),
                        ct
                    ),
                ct
            );
    }
}