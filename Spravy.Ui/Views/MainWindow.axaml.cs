namespace Spravy.Ui.Views;

public partial class MainWindow : SukiWindow, IDesktopTopLevelControl
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        Content = viewModel;
    }
}