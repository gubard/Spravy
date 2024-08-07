namespace Spravy.Ui.Features.ToDo.Views;

public partial class TodayToDoItemsView : UserControl
{
    public TodayToDoItemsView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not TodayToDoItemsView view)
            {
                return;
            }

            if (view.DataContext is not TodayToDoItemsViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
