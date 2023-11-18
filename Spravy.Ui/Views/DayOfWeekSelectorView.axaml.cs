using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class DayOfWeekSelectorView : ReactiveUserControl<DayOfWeekSelectorViewModel>
{
    public DayOfWeekSelectorView()
    {
        InitializeComponent();
    }
}