namespace Spravy.Ui.Features.ToDo.Views;

public partial class LeafToDoItemsView : MainUserControl<LeafToDoItemsViewModel>
{
    public LeafToDoItemsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not LeafToDoItemsView view)
            {
                return;
            }

            if (view.DataContext is not LeafToDoItemsViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(viewModel);
        };
    }
}
