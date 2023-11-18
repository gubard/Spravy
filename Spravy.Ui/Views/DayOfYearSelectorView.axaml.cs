using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class DayOfYearSelectorView : ReactiveUserControl<DayOfYearSelectorViewModel>
{
    public DayOfYearSelectorView()
    {
        InitializeComponent();
    }
}