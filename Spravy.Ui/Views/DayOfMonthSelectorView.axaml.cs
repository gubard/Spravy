using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class DayOfMonthSelectorView : ReactiveUserControl<DayOfMonthSelectorViewModel>
{
    public DayOfMonthSelectorView()
    {
        InitializeComponent();
    }
}