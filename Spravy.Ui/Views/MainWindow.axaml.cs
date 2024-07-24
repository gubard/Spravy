namespace Spravy.Ui.Views;

public partial class MainWindow : Window, IDesktopTopLevelControl
{
    private readonly MainViewModel mainViewModel;

    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        this.mainViewModel = mainViewModel;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        this.GetControl<Panel>("PART_Panel")
            .Children.Add(new ContentControl { Content = mainViewModel, });
    }
}
