using Spravy.Domain.Attributes;
using Spravy.Ui.Controls;

namespace Spravy.Ui.ViewModels;

public class WeeklyPeriodicityViewModel : PeriodicityViewModel
{
    [Inject]
    public required DayOfWeekSelector DayOfWeekSelector { get; init; }
}