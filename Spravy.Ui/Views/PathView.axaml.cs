using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class PathView : ReactiveUserControl<PathViewModel>
{
    public PathView()
    {
        InitializeComponent();
    }
}