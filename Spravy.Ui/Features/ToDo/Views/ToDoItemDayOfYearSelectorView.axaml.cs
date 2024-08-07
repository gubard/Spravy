namespace Spravy.Ui.Features.ToDo.Views;

public partial class ToDoItemDayOfYearSelectorView : UserControl
{
    public ToDoItemDayOfYearSelectorView()
    {
        InitializeComponent();

        Initialized += (s, e) =>
        {
            if (s is not ToDoItemDayOfYearSelectorView view)
            {
                return;
            }

            if (view.DataContext is not ToDoItemDayOfYearSelectorViewModel viewModel)
            {
                return;
            }

            viewModel.InitializedCommand.Command.Execute(null);
        };
    }
}
