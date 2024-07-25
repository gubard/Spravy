namespace Spravy.Ui.ViewModels;

public class SingleViewModel : ViewModelBase
{
    public SingleViewModel(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    public MainViewModel MainViewModel { get; }
}
