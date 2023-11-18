using Ninject;
using ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Models;

public class DayOfYearSelectItem : NotifyBase
{
    private byte month;

    public byte Month
    {
        get => month;
        set => this.RaiseAndSetIfChanged(ref month, value);
    }

    [Inject]
    public required DayOfMonthSelectorViewModel Days { get; init; }
}