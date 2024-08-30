namespace Spravy.Ui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool errorDialogIsOpen;

    [ObservableProperty]
    private bool progressDialogIsOpen;

    [ObservableProperty]
    private bool inputDialogIsOpen;

    [ObservableProperty]
    private bool contentDialogIsOpen;

    [ObservableProperty]
    private IDialogable errorDialogContent = new EmptyDialogable();

    [ObservableProperty]
    private IDialogable progressDialogContent = new EmptyDialogable();

    [ObservableProperty]
    private IDialogable inputDialogContent = new EmptyDialogable();

    [ObservableProperty]
    private IDialogable contentDialogContent = new EmptyDialogable();

    public MainViewModel(MainProgressBarViewModel mainProgressBar, MainSplitViewModel mainSplit)
    {
        MainProgressBar = mainProgressBar;
        MainSplit = mainSplit;
    }

    public MainSplitViewModel MainSplit { get; }
    public MainProgressBarViewModel MainProgressBar { get; }
}
