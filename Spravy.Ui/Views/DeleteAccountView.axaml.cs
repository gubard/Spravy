namespace Spravy.Ui.Views;

public partial class DeleteAccountView : UserControl
{
    public DeleteAccountView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not DeleteAccountView view)
            {
                return;
            }

            if (view.DataContext is not DeleteAccountViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
