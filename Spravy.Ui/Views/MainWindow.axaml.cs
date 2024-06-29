namespace Spravy.Ui.Views;

public partial class MainWindow : SukiWindow, IDesktopTopLevelControl
{
    public MainWindow(ISingleViewTopLevelControl singleView)
    {
        InitializeComponent();
        Content = singleView;
    }
}
