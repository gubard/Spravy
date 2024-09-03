namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemCreateTimerViewModel
    : DialogableViewModelBase,
        IAddTimerParametersFactory
{
    private readonly IObjectStorage objectStorage;
    private readonly ISerializer serializer;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private DateTime date = DateTime.Today;

    [ObservableProperty]
    private TimeSpan time = DateTime.Now.TimeOfDay;

    public ToDoItemCreateTimerViewModel(
        ToDoItemEntityNotify item,
        IObjectStorage objectStorage,
        ISerializer serializer,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Item = item;
        this.objectStorage = objectStorage;
        this.serializer = serializer;
        name = item.Name;

        AddTime = SpravyCommand.Create<TimeSpan>(
            (t, ct) =>
                this.PostUiBackground(
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
    }

    public SpravyCommand AddTime { get; }
    public ToDoItemEntityNotify Item { get; }
    public AvaloniaList<TimeSpan> Times { get; } = new();
    public AvaloniaList<string> Names { get; } = new();

    public override string ViewId
    {
        get => $"{TypeCache<ToDoItemCreateTimerViewModel>.Type}:{Item.Id}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<ToDoItemCreateTimerViewModelSettings>(ViewId, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
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
        return objectStorage.SaveObjectAsync(
            ViewId,
            new ToDoItemCreateTimerViewModelSettings(this),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<AddTimerParameters>> CreateAddTimerParametersAsync(
        CancellationToken ct
    )
    {
        return serializer
            .SerializeAsync(new AddToDoItemToFavoriteEventOptions { ToDoItemId = Item.Id, }, ct)
            .IfSuccessAsync(
                content =>
                    new AddTimerParameters(
                        Date.Date.Add(Time),
                        AddToDoItemToFavoriteEventOptions.EventId,
                        content,
                        Name
                    ).ToResult(),
                ct
            );
    }
}
