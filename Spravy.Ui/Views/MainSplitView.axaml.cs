using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class MainSplitView : ReactiveUserControl<MainSplitViewModel>
{
    public MainSplitView()
    {
        InitializeComponent();
    }
}