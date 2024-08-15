namespace Spravy.Ui.ViewModels;

public partial class MainSplitViewModel(PaneViewModel pane) : ViewModelBase
{
    [ObservableProperty]
    private bool isPaneOpen;

    [ObservableProperty]
    private object? content;

    public PaneViewModel Pane { get; } = pane;
}
