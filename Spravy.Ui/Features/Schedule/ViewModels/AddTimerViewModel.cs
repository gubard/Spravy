using Spravy.Ui.Features.Schedule.Enums;

namespace Spravy.Ui.Features.Schedule.ViewModels;

public partial class AddTimerViewModel : ViewModelBase
{
    private readonly IViewFactory viewFactory;

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

    public AddTimerViewModel(IViewFactory viewFactory)
    {
        this.viewFactory = viewFactory;
        eventViewModel = GetEventViewModel();
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
}
