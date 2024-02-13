using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Ninject;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DayOfYearSelectorViewModel : ViewModelBase, IApplySettings
{
    [Inject]
    public required AvaloniaList<DayOfYearSelectItem> Items { get; init; }

    public Task ApplySettingsAsync(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}