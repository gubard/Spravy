namespace Spravy.Ui.Features.ToDo.Views;

public partial class DeleteToDoItemView : UserControl
{
    public DeleteToDoItemView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not DeleteToDoItemView view)
            {
                return;
            }

            if (view.DataContext is not DeleteToDoItemViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(viewModel);
        };
    }
}
