namespace Spravy.Ui.Views;

public partial class ConfirmView : UserControl
{
    public const string ContentContentControlName = "content-content-control";
    public const string OkButtonName = "ok-button";
    public const string CancelButtonName = "cancel-button";

    public ConfirmView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ConfirmView view)
            {
                return;
            }

            if (view.DataContext is not ConfirmViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
