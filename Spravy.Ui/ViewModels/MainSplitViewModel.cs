namespace Spravy.Ui.ViewModels;

public partial class MainSplitViewModel : ViewModelBase
{
    [ObservableProperty]
    private INavigatable content = new EmptyNavigatable();

    [ObservableProperty]
    private bool isPaneOpen;

    public MainSplitViewModel(PaneViewModel pane)
    {
        Pane = pane;
    }

    public PaneViewModel Pane { get; }
}