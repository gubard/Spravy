namespace Spravy.Ui.Views;

public partial class MainWindow : Window, IDesktopTopLevelControl
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        Content = viewModel;
    }
}
