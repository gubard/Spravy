namespace Spravy.Ui.Views;

public partial class EditDescriptionContentView : UserControl
{
    public EditDescriptionContentView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not EditDescriptionContentView view)
            {
                return;
            }

            if (view.DataContext is not EditDescriptionContentViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(view);
        };
    }
}
