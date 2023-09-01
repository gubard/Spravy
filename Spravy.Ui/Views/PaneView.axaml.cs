using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class PaneView : ReactiveUserControl<PaneViewModel>
{
    public PaneView()
    {
        InitializeComponent();
    }
}