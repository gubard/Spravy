using Ninject;
using Spravy.Ui.Controls;

namespace Spravy.Ui.ViewModels;

public class MonthlyPeriodicityViewModel : PeriodicityViewModel
{
    [Inject]
    public required DayOfMonthSelector DayOfMonthSelector { get; init; }
}