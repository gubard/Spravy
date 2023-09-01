using Avalonia.Collections;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class TimersViewModel : RoutableViewModelBase
{
    public TimersViewModel() : base("timers")
    {
    }

    public AvaloniaList<TimerNotify> Timers { get; } = new();
}