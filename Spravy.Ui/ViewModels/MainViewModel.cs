namespace Spravy.Ui.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private IDialogable contentDialogContent = new EmptyDialogable();

    [ObservableProperty]
    private bool contentDialogIsOpen;

    [ObservableProperty]
    private IDialogable errorDialogContent = new EmptyDialogable();

    [ObservableProperty]
    private bool errorDialogIsOpen;

    [ObservableProperty]
    private IDialogable inputDialogContent = new EmptyDialogable();

    [ObservableProperty]
    private bool inputDialogIsOpen;

    [ObservableProperty]
    private IDialogable progressDialogContent = new EmptyDialogable();

    [ObservableProperty]
    private bool progressDialogIsOpen;

    public MainViewModel(MainProgressBarViewModel mainProgressBar, MainSplitViewModel mainSplit)
    {
        MainProgressBar = mainProgressBar;
        MainSplit = mainSplit;
    }

    public MainSplitViewModel MainSplit { get; }
    public MainProgressBarViewModel MainProgressBar { get; }
}