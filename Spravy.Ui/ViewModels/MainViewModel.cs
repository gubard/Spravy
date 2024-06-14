namespace Spravy.Ui.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(MainProgressBarViewModel mainProgressBar, MainSplitViewModel mainSplit)
    {
        MainProgressBar = mainProgressBar;
        MainSplit = mainSplit;
    }
    
    public MainSplitViewModel MainSplit { get; }
    public MainProgressBarViewModel MainProgressBar { get; }
}