using ExtensionFramework.AvaloniaUi.Controls;
using ExtensionFramework.Core.DependencyInjection.Attributes;

namespace Spravy.Ui.ViewModels;

public class WeeklyPeriodicityViewModel : PeriodicityViewModel
{
    [Inject]
    public required DayOfWeekSelector DayOfWeekSelector { get; init; }
}