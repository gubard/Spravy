using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class MainWindow : ReactiveWindow<MainWindowModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }
}