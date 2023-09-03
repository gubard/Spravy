using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class TimersView : ReactiveUserControl<TimersViewModel>
{
    public TimersView()
    {
        InitializeComponent();
    }
}