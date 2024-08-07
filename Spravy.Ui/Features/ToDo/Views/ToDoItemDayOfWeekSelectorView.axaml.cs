namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemDayOfWeekSelectorView : UserControl
{
    public ToDoItemDayOfWeekSelectorView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ToDoItemDayOfWeekSelectorView view)
            {
                return;
            }

            if (view.DataContext is not ToDoItemDayOfWeekSelectorViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
