using ExtensionFramework.AvaloniaUi.Controls;
using ExtensionFramework.Core.DependencyInjection.Attributes;

namespace Spravy.Ui.ViewModels;

public class MonthlyPeriodicityViewModel : PeriodicityViewModel
{
    [Inject]
    public required DayOfMonthSelector DayOfMonthSelector { get; init; }
}