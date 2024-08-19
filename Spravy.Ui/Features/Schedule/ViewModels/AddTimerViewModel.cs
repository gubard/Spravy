using Spravy.Ui.Features.Schedule.Enums;
using Spravy.Ui.Features.Schedule.Settings;

namespace Spravy.Ui.Features.Schedule.ViewModels;

public partial class AddTimerViewModel : ViewModelBase, INavigatable
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

    public AddTimerViewModel(IViewFactory viewFactory, IObjectStorage objectStorage)
    {
        this.viewFactory = viewFactory;
        this.objectStorage = objectStorage;
        eventViewModel = GetEventViewModel();
    }

    public bool IsPooled => false;
    public AvaloniaList<string> Names { get; } = new();

    public string ViewId
    {
        get => $"{TypeCache<AddTimerViewModel>.Type}";
    }

    private IEventViewModel GetEventViewModel()
    {
        return Type switch
        {
            EventType.AddToDoItemToFavorite
                => viewFactory.CreateAddToDoItemToFavoriteEventViewModel(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public ConfiguredValueTaskAwaitable<Result<AddTimerParameters>> ToAddTimerParametersAsync(
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

    public Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<AddTimerViewModelSettings>(ViewId, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            Name = setting.Name;
                            Names.AddRange(setting.Names);

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }

    public Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new AddTimerViewModelSettings(this), ct);
    }

    public Result Stop()
    {
        return Result.Success;
    }
}
