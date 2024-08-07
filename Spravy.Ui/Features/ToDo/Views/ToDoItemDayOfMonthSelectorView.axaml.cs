namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemDayOfMonthSelectorView : UserControl
{
    public ToDoItemDayOfMonthSelectorView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ToDoItemDayOfMonthSelectorView view)
            {
                return;
            }

            if (view.DataContext is not ToDoItemDayOfMonthSelectorViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
