namespace Spravy.Ui.Views;

public partial class MainSplitView : UserControl
{
    public const string MainSplitViewName = "main-split-view";

    public MainSplitView()
    {
        InitializeComponent();
        Initialized += (_, _) => UiHelper.MainSplitViewModelInitialized.Execute(null);
    }
}
