namespace Spravy.Ui.ViewModels;

public class MainViewModel : ViewModelBase
{
    [Inject]
    public required MainSplitViewModel MainSplit { get; init; }
    
    [Inject]
    public required MainProgressBarViewModel MainProgressBar { get; init; }
}