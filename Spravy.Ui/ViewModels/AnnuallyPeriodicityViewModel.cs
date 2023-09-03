using Ninject;
using Spravy.Ui.Controls;

namespace Spravy.Ui.ViewModels;

public class AnnuallyPeriodicityViewModel : PeriodicityViewModel
{
    [Inject]
    public required DayOfYearSelector DayOfYearSelector { get; init; }
}