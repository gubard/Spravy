using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DayOfMonthSelectorViewModel : ViewModelBase, IApplySettings
{
    public DayOfMonthSelectorViewModel()
    {
        Items = new(
            Enumerable.Range(1, 31)
                .Select(
                    x => new DayOfMonthSelectItem
                    {
                        Day = (byte)x
                    }
                )
        );
    }

    public AvaloniaList<DayOfMonthSelectItem> Items { get; }

    public Task ApplySettingsAsync(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}