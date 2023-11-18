using Avalonia.Collections;
using Ninject;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DayOfYearSelectorViewModel : ViewModelBase
{
    [Inject]
    public required AvaloniaList<DayOfYearSelectItem> Items { get; init; }
}