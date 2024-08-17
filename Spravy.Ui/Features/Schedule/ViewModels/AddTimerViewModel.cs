namespace Spravy.Ui.Features.Schedule.ViewModels;

public partial class AddTimerViewModel : ViewModelBase
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private DateTime date = DateTime.Today;

    [ObservableProperty]
    private TimeSpan time = DateTime.Now.TimeOfDay;
}
