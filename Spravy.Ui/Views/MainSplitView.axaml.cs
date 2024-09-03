namespace Spravy.Ui.Views;

public partial class MainSplitView : UserControl
{
    public MainSplitView()
    {
        InitializeComponent();
        Initialized += (_, _) => UiHelper.MainSplitViewModelInitialized.Execute(null);
    }
}
