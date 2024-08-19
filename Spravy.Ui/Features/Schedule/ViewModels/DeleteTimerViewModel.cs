namespace Spravy.Ui.Features.Schedule.ViewModels;

public class DeleteTimerViewModel : ViewModelBase
{
    public DeleteTimerViewModel(TimerItemNotify item)
    {
        Item = item;
    }

    public TimerItemNotify Item { get; }
}
