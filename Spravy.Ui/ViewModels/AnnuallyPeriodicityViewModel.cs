using ExtensionFramework.AvaloniaUi.Controls;
using ExtensionFramework.Core.DependencyInjection.Attributes;

namespace Spravy.Ui.ViewModels;

public class AnnuallyPeriodicityViewModel : PeriodicityViewModel
{
    [Inject]
    public required DayOfYearSelector DayOfYearSelector { get; init; }
}