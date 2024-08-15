namespace Spravy.Ui.Features.ToDo.Views;

public partial class AddToDoItemView : UserControl
{
    public AddToDoItemView()
    {
        InitializeComponent();

        Initialized += (_, _) =>
        {
            if (DataContext is not AddToDoItemViewModel viewModel)
            {
                return;
            }

            viewModel.Initialized.Command.Execute(null);
        };
    }
}
