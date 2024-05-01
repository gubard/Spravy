namespace Spravy.Ui.ViewModels;

public class CalendarViewModel : ViewModelBase
{
    [Reactive]
    public DateTime? SelectedDate { get; set; }
}