namespace Spravy.Ui.ViewModels;

public class MainWindowModel : ViewModelBase
{
    public MainWindowModel(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public MainViewModel MainViewModel { get; }
}