namespace Spravy.Ui.ViewModels;

public partial class MainSplitViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isPaneOpen;

    [ObservableProperty]
    private INavigatable content = new EmptyNavigatable();

    public MainSplitViewModel(PaneViewModel pane)
    {
        Pane = pane;
    }

    public PaneViewModel Pane { get; }
}
