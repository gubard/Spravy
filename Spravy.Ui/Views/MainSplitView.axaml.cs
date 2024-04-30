using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class MainSplitView : ReactiveUserControl<MainSplitViewModel>
{
    public const string MainSplitViewName = "main-split-view";

    public MainSplitView()
    {
        InitializeComponent();
    }
}