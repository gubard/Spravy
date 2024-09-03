using Spravy.Ui.Features.Schedule.Enums;
using Spravy.Ui.Features.Schedule.Settings;

namespace Spravy.Ui.Features.Schedule.ViewModels;

public partial class AddTimerViewModel : DialogableViewModelBase, IAddTimerParametersFactory
{
    private readonly IViewFactory viewFactory;
    private readonly IObjectStorage objectStorage;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private DateTime date = DateTime.Today;

    [ObservableProperty]
    private TimeSpan time = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    private EventType type;

    [ObservableProperty]
    private IEventViewModel eventViewModel;

    public AddTimerViewModel(
        IViewFactory viewFactory,
        IObjectStorage objectStorage,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.viewFactory = viewFactory;
        this.objectStorage = objectStorage;
        eventViewModel = GetEventViewModel();

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
    public AvaloniaList<string> Names { get; } = new();
    public AvaloniaList<TimeSpan> Times { get; } = new();

    public override string ViewId
    {
        get => $"{TypeCache<AddTimerViewModel>.Type}";
    }

    private IEventViewModel GetEventViewModel()
    {
        return Type switch
        {
            EventType.AddToDoItemToFavorite
                => viewFactory.CreateAddToDoItemToFavoriteEventViewModel(),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public ConfiguredValueTaskAwaitable<Result<AddTimerParameters>> CreateAddTimerParametersAsync(
        CancellationToken ct
    )
    {
        return EventViewModel
            .GetContentAsync(ct)
            .IfSuccessAsync(
                content =>
                    new AddTimerParameters(
                        Date.Date.Add(Time),
                        EventViewModel.Id,
                        content,
                        Name
                    ).ToResult(),
                ct
            );
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return EventViewModel
            .LoadStateAsync(ct)
            .IfSuccessAsync(
                () => objectStorage.GetObjectOrDefaultAsync<AddTimerViewModelSettings>(ViewId, ct),
                ct
            )
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            Name = setting.Name;
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
        return objectStorage.SaveObjectAsync(ViewId, new AddTimerViewModelSettings(this), ct);
    }
}
