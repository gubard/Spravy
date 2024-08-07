namespace Spravy.Ui.Views;

public partial class MainSplitView : UserControl
{
    public const string MainSplitViewName = "main-split-view";

    public MainSplitView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not MainSplitView view)
            {
                return;
            }

            if (view.DataContext is not MainSplitViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
