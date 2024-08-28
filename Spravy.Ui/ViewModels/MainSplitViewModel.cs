namespace Spravy.Ui.ViewModels;

public partial class MainSplitViewModel(PaneViewModel pane) : ViewModelBase
{
    [ObservableProperty]
    private bool isPaneOpen;

    [ObservableProperty]
    private INavigatable content = new EmptyNavigatable();

    public PaneViewModel Pane { get; } = pane;
}
